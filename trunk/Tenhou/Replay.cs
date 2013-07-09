using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;

namespace TenhouViewer.Tenhou
{
    class Replay
    {
        private Mahjong.Replay R = new Mahjong.Replay();
        private WallGenerator Generator;
        private int GameIndex = 0;

        private Mahjong.Round CurrentRound;

        public Replay()
        {

        }

        // .xml
        public void OpenPlainText(string Filename)
        {
            XmlReader Reader = XmlReader.Create(Filename);

            Parse(Reader);
        }

        // .mjlog
        public void OpenGZ(string Filename)
        {
            FileStream File = new FileStream(Filename, FileMode.Open, FileAccess.Read);
            GZipStream Stream = new GZipStream(File, CompressionMode.Decompress);
            XmlReader Reader = XmlReader.Create(Stream);

            Parse(Reader);
        }

        private void Parse(XmlReader Reader)
        {
            GameIndex = 0;

            Reader.MoveToContent();
            while (Reader.Read())
            {
                if (Reader.NodeType == XmlNodeType.Element)
                {
                    switch (Reader.Name)
                    {
                    case "GO":
                        // Start of round
                        // Lobby info, type info
                        GO(Reader);
                        break;
                    case "UN":
                        // Player info
                        // Player reconnect
                        UN(Reader);
                        break;
                    case "BYE":
                        // Player goes offline
                        BYE(Reader);
                        break;
                    case "SHUFFLE":
                        // Seed for generating walls]
                        SHUFFLE(Reader);
                        break;
                    case "INIT":
                        // Init game: hands
                        INIT(Reader);
                        break;
                    case "TAIKYOKU":
                        // Current round
                        TAIKYOKU(Reader);
                        break;
                    case "RYUUKYOKU":
                        // Draw
                        RYUUKYOKU(Reader);
                        break;
                    case "N":
                        // Naki - open set
                        N(Reader);
                        break;
                    case "DORA":
                        // Open new dora indicator
                        DORA(Reader);
                        break;
                    case "AGARI":
                        // Ron or Tsumo
                        AGARI(Reader);
                        break;
                    case "REACH":
                        // declare riichi! 2 steps
                        REACH(Reader);
                        break;
                    default:
                        // Action: draw and discard tile
                        ACTION(Reader);
                        break;
                    }
                }
            }
        }

        private void GO(XmlReader Reader)
        {
            int Lobby = Convert.ToInt16(Reader.GetAttribute("lobby"));
        }

        private void UN(XmlReader Reader)
        {
            string Dan = Reader.GetAttribute("dan");
            string Rate = Reader.GetAttribute("rate");
            string Sex = Reader.GetAttribute("sx");

            int[] DanList = DecompositeIntList(Dan);
            int[] RateList = DecompositeIntList(Rate);
            string[] SexList = DecompositeStringList(Sex);

            for (int i = 0; i < 4; i++)
            {
                string NickName = Reader.GetAttribute("n" + i.ToString());

                if (NickName != null) NickName = Uri.UnescapeDataString(NickName);
            }
        }
        
        private void BYE(XmlReader Reader)
        {
            // Player goes offline
            int Who = Convert.ToInt16(Reader.GetAttribute("who"));
        }

        private void SHUFFLE(XmlReader Reader)
        {
            string Seed = Reader.GetAttribute("seed");

            Generator = new WallGenerator(Seed);
        }

        private void INIT(XmlReader Reader)
        {
            Mahjong.Wall Wall = new Mahjong.Wall();
            Mahjong.Hand[] Hands = new Mahjong.Hand[4];

            // Generate wall
            Generator.Generate(GameIndex);

            // Balance
            int[] Balance = DecompositeIntList(Reader.GetAttribute("ten"));

            for (int i = 0; i < 4; i++)
            {
                // Tile list, 13 tiles
                int[] TileList = DecompositeIntList(Reader.GetAttribute("hai" + i.ToString()));

                Hands[i] = new Mahjong.Hand();
                Hands[i].SetArray(TileList);
            }

            // Start new round!
            CurrentRound = new Mahjong.Round();
            CurrentRound.SetStartHands(Hands);
        }

        private void TAIKYOKU(XmlReader Reader)
        {

        }

        private void RYUUKYOKU(XmlReader Reader)
        {
            // Draw
        }

        private void N(XmlReader Reader)
        {
            // Naki
            uint Who = Convert.ToUInt16(Reader.GetAttribute("who"));
            uint m = Convert.ToUInt16(Reader.GetAttribute("m"));

        }

        private void DORA(XmlReader Reader)
        {
            // Open dora indicator
        }

        private void AGARI(XmlReader Reader)
        {
            // Ron or tsumo!
        }

        private void REACH(XmlReader Reader)
        {
            // Riichi!
            uint Who = Convert.ToUInt16(Reader.GetAttribute("who"));
            uint Step = Convert.ToUInt16(Reader.GetAttribute("step"));

            // Step 1: declare riichi
            // discard tile
            // Step 2: pay 1k
        }

        private void ACTION(XmlReader Reader)
        {
            // Step!
            string Value = Reader.Name.Substring(1);
            uint Tile;

            try
            {
                Tile = Convert.ToUInt16(Value);
            }
            catch (Exception E)
            {
                return;
            }

            switch (Reader.Name[0])
            {
                case 'T': // first player draw tile
                    break;
                case 'D': // first player discard tile
                    break;
                case 'U': // second player draw tile
                    break;
                case 'E': // second player discard tile
                    break;
                case 'V': // third player draw tile
                    break;
                case 'F': // third player discard tile
                    break;
                case 'W': // fourth player draw tile
                    break;
                case 'G': // fourth player discard tile
                    break;
            }
        }

        private int[] DecompositeIntList(string Text)
        {
            string[] delimiter = new string[] { "," };
            string[] Temp;
            int[] Result = null;

            if (Text == null) return null;

            Temp = Text.Split(delimiter, StringSplitOptions.None);
            Result = new int[Temp.Length];

            for (int i = 0; i < Temp.Length; i++)
            {
                // Отрежем текст до точки
                int Index = Temp[i].IndexOf('.');
                if(Index >= 0) Temp[i] = Temp[i].Substring(0, Index);

                Result[i] = Convert.ToUInt16(Temp[i]);
            }

            return Result;
        }

        private string[] DecompositeStringList(string Text)
        {
            string[] delimiter = new string[] { "," };

            if (Text == null) return null;

            return Text.Split(delimiter, StringSplitOptions.None);
        }
    }
}

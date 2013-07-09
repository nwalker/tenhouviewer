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
        Mahjong.Replay R = new Mahjong.Replay();
        WallGenerator Generator;
        int GameIndex = 0;

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
                        break;
                    case "UN":
                        // Player info
                        // Player reconnect
                        UN(Reader);
                        break;
                    case "BYE":
                        // Player goes offline
                        break;
                    case "SHUFFLE":
                        // Seed for generating walls]
                        SHUFFLE(Reader);
                        break;
                    case "INIT":
                        // Init game: hands
                        break;
                    case "TAIKYOKU":
                        // Current round
                        break;
                    case "RYUUKYOKU":
                        // Draw
                        break;
                    case "N":
                        // Naki - open set
                        break;
                    case "DORA":
                        // Open new dora indicator
                        break;
                    case "AGARI":
                        // Ron or Tsumo
                        break;
                    case "REACH":
                        // declare riichi! 2 steps
                        break;
                    default:
                        // Action: draw and discard tile
                        break;
                    }
                }
            }
        }

        private void SHUFFLE(XmlReader Reader)
        {
            string Seed = Reader.GetAttribute("seed");

            Generator = new WallGenerator(Seed);
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

        private void INIT(XmlReader Reader)
        {
            Mahjong.Wall Wall = new Mahjong.Wall();
            Mahjong.Hand[] Hands = new Mahjong.Hand[4];

            // Generate wall
            Generator.Generate(GameIndex);

            for (int i = 0; i < 4; i++)
            {
                string HandString = Reader.GetAttribute("hai" + i.ToString());

                // Tile list, 13 tiles
                int[] TileList = DecompositeIntList(HandString);

                Hands[i] = new Mahjong.Hand();
                Hands[i].SetArray(TileList);
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

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;

namespace TenhouViewer.Tenhou
{
    class ReplayDecoder
    {
        private string[] delimiter = new string[] { "," };

        public Mahjong.Replay R = new Mahjong.Replay();
        private WallGenerator Generator;
        private bool FirstStep;
        private int LastDealer = -1;
        private int Round = -1;

        private Mahjong.Round CurrentRound;

        public ReplayDecoder()
        {

        }

        // .xml
        public void OpenPlainText(string Filename, string Hash)
        {
            R.Hash = Hash;

            XmlReader Reader = XmlReader.Create(Filename);
            Parse(Reader);
        }

        // .mjlog
        public void OpenGZ(string Filename, string Hash)
        {
            R.Hash = Hash;

            FileStream File = new FileStream(Filename, FileMode.Open, FileAccess.Read);
            GZipStream Stream = new GZipStream(File, CompressionMode.Decompress);
            XmlReader Reader = XmlReader.Create(Stream);

            Parse(Reader);
        }

        private void Parse(XmlReader Reader)
        {
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
            R.Lobby = Convert.ToInt32(Reader.GetAttribute("lobby"));
            R.LobbyType = (Mahjong.LobbyType)Convert.ToInt32(Reader.GetAttribute("type"));
        }

        // Player info
        private void UN(XmlReader Reader)
        {
            string Dan = Reader.GetAttribute("dan");
            string Rate = Reader.GetAttribute("rate");
            string Sex = Reader.GetAttribute("sx");

            if (Dan == null)
            {
                // player reconnect
            }
            else
            {
                // game start
                int[] DanList = DecompositeIntList(Dan);
                int[] RateList = DecompositeIntList(Rate);
                string[] SexList = DecompositeStringList(Sex);

                Mahjong.Player[] Players = new Mahjong.Player[4];

                for (int i = 0; i < 4; i++)
                {
                    string NickName = Reader.GetAttribute("n" + i.ToString());

                    if (NickName != null) NickName = Uri.UnescapeDataString(NickName);

                    R.Players[i] = new Mahjong.Player();
                    R.Players[i].NickName = NickName;
                    R.Players[i].Rank = DanList[i];
                    R.Players[i].Rating = RateList[i];
                    R.Players[i].Sex = (SexList[i].CompareTo("M") == 0) ? Mahjong.Sex.Male : ((SexList[i].CompareTo("C") == 0) ? Mahjong.Sex.Computer : Mahjong.Sex.Female);
                }
            }
        }
        
        private void BYE(XmlReader Reader)
        {
            // Player goes offline
            int Who = Convert.ToInt16(Reader.GetAttribute("who"));

            Mahjong.Step Step = new Mahjong.Step(Who);
            Step.Disconnect();

            if(CurrentRound != null) CurrentRound.Steps.Add(Step);
        }

        private void SHUFFLE(XmlReader Reader)
        {
            string Seed = Reader.GetAttribute("seed");

            Generator = new WallGenerator(Seed);
        }

        private void INIT(XmlReader Reader)
        {
            R.PlayerCount = 4;

            // Start new round!
            CurrentRound = new Mahjong.Round();
            CurrentRound.Hash = R.Hash;
            CurrentRound.Index = R.Rounds.Count;
            CurrentRound.Lobby = R.Lobby;
            CurrentRound.LobbyType = R.LobbyType;

            R.Rounds.Add(CurrentRound);

            // Generate wall
            if (Generator != null)
            {
                Generator.Generate(CurrentRound.Index);

                CurrentRound.Wall = new Mahjong.Wall();
                CurrentRound.Wall.Tiles = Generator.GetWall();
                CurrentRound.Wall.Dice = Generator.GetDice();
            }

            // Balance
            int[] Balance = DecompositeIntList(Reader.GetAttribute("ten"));

            for (int i = 0; i < 4; i++)
            {
                string Hai = Reader.GetAttribute("hai" + i.ToString());

                if ((Hai.CompareTo("") == 0) && (i == 3))
                {
                    R.PlayerCount = 3;

                    break;
                }

                // Tile list, 13 tiles
                int[] TileList = DecompositeIntList(Hai);

                CurrentRound.StartHands[i] = new Mahjong.Hand();
                CurrentRound.StartHands[i].SetArray(TileList);

                CurrentRound.BalanceBefore[i] = Balance[i] * 100;
            }

            int[] Seed = DecompositeIntList(Reader.GetAttribute("seed"));

            {
                if (Seed != null)
                {
                    // Seed:
                    // 0: unk           3: unk
                    // 1: renchan stick 4: unk
                    // 2: riichi stick  5: dora pointer
                    CurrentRound.RenchanStick = Seed[1];
                    CurrentRound.RiichiStick = Seed[2];
                    CurrentRound.FirstDora = Seed[5];
                }
            }

            CurrentRound.PlayerCount = R.PlayerCount;

            FirstStep = true;
        }

        private void TAIKYOKU(XmlReader Reader)
        {
            // Определение дилера (лол, oya=0)
        }

        private void RYUUKYOKU(XmlReader Reader)
        {
            // Draw
            Mahjong.Step Step = new Mahjong.Step(-1);

            string RyuuType = Reader.GetAttribute("type");
            int Reason = -1;

            switch (RyuuType)
            {
                case "yao9": // 9 terminals/honors
                    Reason = 0;
                    break;
                case "reach4": // 4 consecutive riichi calls
                    Reason = 1;
                    break;
                case "ron3": // three simultaneous ron calls (triple ron)
                    Reason = 2;
                    break;
                case "kan4": // four declared kans
                    Reason = 3;
                    break;
                case "kaze4": // same wind discard on first round
                    Reason = 4;
                    break;
                case "nm": // nagashi mangan (all terminal/honor discards)
                    Reason = 5;
                    break;
            }

            Step.Draw(Reason);
            CurrentRound.Steps.Add(Step);
            CurrentRound.Result = Mahjong.RoundResult.Draw;
            CurrentRound.DrawReason = Reason;

            CheckScore(Reader);
            CheckEnd(Reader);
        }

        private void N(XmlReader Reader)
        {
            // Naki
            int Who = Convert.ToInt32(Reader.GetAttribute("who"));
            int m = Convert.ToInt32(Reader.GetAttribute("m"));

            NakiDecoder Naki = new NakiDecoder(m);
            Mahjong.Step Step = new Mahjong.Step(Who);

            Step.Naki(Naki.GetNaki());

            CurrentRound.Steps.Add(Step);

            CurrentRound.Naki[Who]++;
            // Ignore chakan and nuki naki
            if (!((Step.NakiData.Type == Mahjong.NakiType.CHAKAN) || (Step.NakiData.Type == Mahjong.NakiType.NUKI)))
                CurrentRound.OpenedSets[Who]++;
        }

        private void DORA(XmlReader Reader)
        {
            // Open dora indicator
            Mahjong.Step Step = new Mahjong.Step(-1);

            int Hai = Convert.ToInt32(Reader.GetAttribute("hai"));

            Step.NewDora(Hai);
            CurrentRound.Steps.Add(Step);

            CurrentRound.DoraCount++;
        }

        private void AGARI(XmlReader Reader)
        {
            // Ron or tsumo!
            int Who = Convert.ToInt16(Reader.GetAttribute("who"));
            int fromWho = Convert.ToInt16(Reader.GetAttribute("fromWho"));

            Mahjong.Step Step = new Mahjong.Step(Who);

            if (Who == fromWho)
            {
                // Tsumo!
                Step.Tsumo();
                CurrentRound.Steps.Add(Step);

                CurrentRound.Result = Mahjong.RoundResult.Tsumo;
            }
            else
            {
                // Ron!
                Step.Ron(fromWho);
                CurrentRound.Steps.Add(Step);

                CurrentRound.Result = Mahjong.RoundResult.Ron;
                CurrentRound.Loser[fromWho] = true;
            }

            CurrentRound.Winner[Who] = true;

            // Sticks count
            {
                int[] Ba = DecompositeIntList(Reader.GetAttribute("ba"));

               // CurrentRound.RenchanStick = Ba[0];
               // CurrentRound.RiichiStick = Ba[1];
            }

            // Hand and waiting
            {
                // hai - tiles
                // machi - waiting
                string Hai = Reader.GetAttribute("hai");
                int Machi = Convert.ToInt16(Reader.GetAttribute("machi"));
            }

            // Dora
            {
                int[] Dora = DecompositeIntList(Reader.GetAttribute("doraHai"));
                int[] UraDora = DecompositeIntList(Reader.GetAttribute("doraHaiUra"));

                for (int i = 0; i < Dora.Length; i++) CurrentRound.Dora.Add(Dora[i]);

                if(UraDora != null)
                    for (int i = 0; i < UraDora.Length; i++) CurrentRound.UraDora.Add(UraDora[i]);
            }

            // Hand cost
            {
                int[] Ten = DecompositeIntList(Reader.GetAttribute("ten"));

                int Fu = Ten[0];
                int Cost = Ten[1];

                CurrentRound.FuCount[Who] = Fu;
                CurrentRound.Cost[Who] = Cost;
            }

            // Yaku list
            {
                bool Yakuman = false;
                string YakuList = Reader.GetAttribute("yaku");
                if (YakuList == null)
                {
                    YakuList = Reader.GetAttribute("yakuman");
                    Yakuman = true;
                }

                int[] Yaku = DecompositeIntList(YakuList);

                if (Yakuman)
                {
                    for (int i = 0; i < Yaku.Length; i++)
                    {
                        int YakuIndex = Yaku[i];
                        int YakuCost = 13;

                        CurrentRound.HanCount[Who] += YakuCost;
                        CurrentRound.Yaku[Who].Add(new Mahjong.Yaku(YakuIndex, YakuCost));
                    }
                }
                else
                {
                    for (int i = 0; i < Yaku.Length / 2; i++)
                    {
                        int YakuIndex = Yaku[i * 2];
                        int YakuCost = Yaku[i * 2 + 1];

                        if (YakuCost > 0)
                        {
                            CurrentRound.HanCount[Who] += YakuCost;
                            CurrentRound.Yaku[Who].Add(new Mahjong.Yaku(YakuIndex, YakuCost));
                        }
                    }
                }
            }

            CheckScore(Reader);
            CheckEnd(Reader);
        }

        private void CheckEnd(XmlReader Reader)
        {
            string owari = Reader.GetAttribute("owari");

            if (owari == null) return;

            // sample: -3,-50.0,183,-22.0,190,-1.0,630,73.0
            int[] Temp = DecompositeIntList(owari);

            for (int i = 0; i < R.PlayerCount; i++)
            {
                R.Balance[i] = Temp[i * 2] * 100;
                R.Result[i] = Temp[i * 2 + 1];
            }

            // Calculate places
            int[] ListOrder = new int[R.PlayerCount];

            for (int i = 0; i < R.PlayerCount; i++) ListOrder[i] = R.Result[i];

            Array.Sort(ListOrder);
            Array.Reverse(ListOrder);

            for (int i = 0; i < R.PlayerCount; i++)
            {
                int Index = 0;
                for (int j = 0; j < R.PlayerCount; j++) if (ListOrder[i] == R.Result[j]) Index = j;

                R.Place[Index] = i + 1;
            }
        }

        private void CheckScore(XmlReader Reader)
        {
            string score = Reader.GetAttribute("sc");

            if (score == null) return;
            string[] Temp = score.Split(delimiter, StringSplitOptions.None);

            for (int i = 0; i < R.PlayerCount; i++)
            {
                int Balance = Convert.ToInt16(Temp[i * 2 + 0]) * 100;
                int Pay = Convert.ToInt16(Temp[i * 2 + 1]) * 100;

                CurrentRound.Pay[i] += Pay;
                CurrentRound.BalanceAfter[i] = Balance + Pay;
            }
        }

        private void REACH(XmlReader Reader)
        {
            // Riichi!
            int Who = Convert.ToInt16(Reader.GetAttribute("who"));
            int S = Convert.ToInt16(Reader.GetAttribute("step"));
            Mahjong.Step Step = new Mahjong.Step(Who);

            switch (S)
            {
                case 1: // declare riichi
                    Step.DeclareRiichi();
                    break;
                case 2: // pay 1k
                    Step.PayRiichi();
                    CurrentRound.Riichi[Who] = CurrentRound.Steps.Count + 1;
                    break;
            }

            CurrentRound.Steps.Add(Step);
        }

        /// <summary>
        /// Игрок взял тайл со стены
        /// </summary>
        /// <param name="Player"></param>
        /// <param name="Tile"></param>
        /// <returns></returns>
        private Mahjong.Step DrawTilePlayer(int Player, int Tile)
        {
            var Step = new Mahjong.Step(Player);
            Step.DrawTile(Tile);

            return Step;
        }

        /// <summary>
        /// Игрок сбросил тайл
        /// </summary>
        /// <param name="Player"></param>
        /// <param name="Tile"></param>
        /// <returns></returns>
        private Mahjong.Step DiscardTilePlayer(int Player, int Tile)
        {
            var Step = new Mahjong.Step(Player);
            Step.DiscardTile(Tile);
            CurrentRound.StepCount[Player]++;
            CheckDealer(Player);
            return Step;
        }

        private void ACTION(XmlReader Reader)
        {
            // Step!
            Mahjong.Step Step = null;
            string Value = Reader.Name.Substring(1);
            int Tile;

            try
            {
                Tile = Convert.ToInt16(Value);
            }
            catch (Exception)
            {
                return;
            }

            switch (Reader.Name[0])
            {
                case 'T': Step = DrawTilePlayer(0, Tile); break;
                case 'D': Step = DiscardTilePlayer(0, Tile); break;
                case 'U': Step = DrawTilePlayer(1, Tile); break;
                case 'E': Step = DiscardTilePlayer(1, Tile); break;
                case 'V': Step = DrawTilePlayer(2, Tile); break;
                case 'F': Step = DiscardTilePlayer(2, Tile); break;
                case 'W': Step = DrawTilePlayer(3, Tile); break;
                case 'G': Step = DiscardTilePlayer(3, Tile); break;
                default: return;
            }

            CurrentRound.Steps.Add(Step);
        }

        private void CheckDealer(int Player)
        {
            if (FirstStep)
            {
                int Index, i;

                // Select dealer
                FirstStep = false;
                CurrentRound.Dealer[Player] = true;
                if (Player != LastDealer) Round++;

                CurrentRound.CurrentRound = Round;

                // Set winds to players
                LastDealer = Player;
                Index = 0;
                for (i = 0; i < CurrentRound.PlayerCount; i++)
                {
                    CurrentRound.Wind[Player] = Index;

                    Player++;
                    Index++;
                    if (Player >= CurrentRound.PlayerCount) Player = 0;
                }
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

                Result[i] = Convert.ToInt32(Temp[i]);
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

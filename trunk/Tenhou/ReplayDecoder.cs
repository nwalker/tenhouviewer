﻿using System;
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
        private int GameIndex = 0;
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
            int Lobby = Convert.ToInt32(Reader.GetAttribute("lobby"));
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
                    R.Players[i].Sex = (SexList[i].CompareTo("M") == 0) ? Mahjong.Sex.Male : Mahjong.Sex.Female;
                }
            }
        }
        
        private void BYE(XmlReader Reader)
        {
            // Player goes offline
            int Who = Convert.ToInt16(Reader.GetAttribute("who"));

            Mahjong.Step Step = new Mahjong.Step(Who);
            Step.Disconnect();

            CurrentRound.Steps.Add(Step);
        }

        private void SHUFFLE(XmlReader Reader)
        {
            string Seed = Reader.GetAttribute("seed");

            Generator = new WallGenerator(Seed);
        }

        private void INIT(XmlReader Reader)
        {
            // Start new round!
            CurrentRound = new Mahjong.Round();
            CurrentRound.Wall = new Mahjong.Wall();
            CurrentRound.Hash = R.Hash;
            CurrentRound.Index = R.Rounds.Count;

            R.Rounds.Add(CurrentRound);

            // Generate wall
            Generator.Generate(GameIndex);

            // Balance
            int[] Balance = DecompositeIntList(Reader.GetAttribute("ten"));

            for (int i = 0; i < 4; i++)
            {
                // Tile list, 13 tiles
                int[] TileList = DecompositeIntList(Reader.GetAttribute("hai" + i.ToString()));

                CurrentRound.StartHands[i] = new Mahjong.Hand();
                CurrentRound.StartHands[i].SetArray(TileList);

                CurrentRound.BalanceBefore[i] = Balance[i] * 100;
            }

            FirstStep = true;
        }

        private void TAIKYOKU(XmlReader Reader)
        {

        }

        private void RYUUKYOKU(XmlReader Reader)
        {
            // Draw
            Mahjong.Step Step = new Mahjong.Step(-1);

            Step.Draw(0);
            CurrentRound.Steps.Add(Step);
            CurrentRound.Result = Mahjong.RoundResult.Draw;

            CheckScore(Reader);
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
            CurrentRound.OpenedSets[Who]++;
        }

        private void DORA(XmlReader Reader)
        {
            // Open dora indicator
            Mahjong.Step Step = new Mahjong.Step(-1);

            //Step.NewDora(???);
            //CurrentRound.Steps.Add(Step);
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

                CurrentRound.RenchanStick = Ba[0];
                CurrentRound.RiichiStick = Ba[1];
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

            for (int i = 0; i < 4; i++)
            {
                R.Balance[i] = Temp[i * 2 + 1];
                R.Result[i] = Temp[i * 2] * 100;
            }

            // Calculate places
            int[] ListOrder = new int[4];

            for (int i = 0; i < 4; i++) ListOrder[i] = R.Balance[i];

            Array.Sort(ListOrder);
            Array.Reverse(ListOrder);

            for (int i = 0; i < 4; i++)
            {
                int Index = 0;
                for (int j = 0; j < 4; j++) if (ListOrder[i] == R.Balance[j]) Index = j;

                R.Place[Index] = i + 1;
            }
        }

        private void CheckScore(XmlReader Reader)
        {
            string score = Reader.GetAttribute("sc");

            if (score == null) return;
            string[] Temp = score.Split(delimiter, StringSplitOptions.None);

            for (int i = 0; i < 4; i++)
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
            catch (Exception E)
            {
                return;
            }

            switch (Reader.Name[0])
            {
                case 'T': // first player draw tile
                    Step = new Mahjong.Step(0);
                    Step.DrawTile(Tile);
                    break;
                case 'D': // first player discard tile
                    Step = new Mahjong.Step(0);
                    Step.DiscardTile(Tile);
                    CurrentRound.StepCount[0]++;
                    CheckDealer(0);
                    break;
                case 'U': // second player draw tile
                    Step = new Mahjong.Step(1);
                    Step.DrawTile(Tile);
                    break;
                case 'E': // second player discard tile
                    Step = new Mahjong.Step(1);
                    Step.DiscardTile(Tile);
                    CurrentRound.StepCount[1]++;
                    CheckDealer(1);
                    break;
                case 'V': // third player draw tile
                    Step = new Mahjong.Step(2);
                    Step.DrawTile(Tile);
                    break;
                case 'F': // third player discard tile
                    Step = new Mahjong.Step(2);
                    Step.DiscardTile(Tile);
                    CurrentRound.StepCount[2]++;
                    CheckDealer(2);
                    break;
                case 'W': // fourth player draw tile
                    Step = new Mahjong.Step(3);
                    Step.DrawTile(Tile);
                    break;
                case 'G': // fourth player discard tile
                    Step = new Mahjong.Step(3);
                    Step.DiscardTile(Tile);
                    CurrentRound.StepCount[3]++;
                    CheckDealer(3);
                    break;
                default:
                    return;
            }

            CurrentRound.Steps.Add(Step);
        }

        private void CheckDealer(int Player)
        {
            if (FirstStep)
            {
                FirstStep = false;
                CurrentRound.Dealer[Player] = true;
                if (Player != LastDealer) Round++;

                CurrentRound.CurrentRound = Round;

                LastDealer = Player;
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

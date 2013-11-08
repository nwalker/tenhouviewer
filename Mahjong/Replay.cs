using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace TenhouViewer.Mahjong
{
    class Replay
    {
        public int PlayerCount = 4;
        public string Hash = "";
        public Player[] Players = new Player[4];
        public List<Round> Rounds = new List<Round>();
        public List<string> RoundFiles = new List<string>();

        public DateTime Date;

        public int[] Result = new int[4];
        public int[] Balance = new int[4];
        public int[] Place = new int[4];

        public int Lobby = -1;
        public int LobbyType = -1;

        public Replay()
        {

        }

        public void Save()
        {
            if (!Directory.Exists("replay")) Directory.CreateDirectory("replay");

            SaveXml(GetFileName(Hash));
            // Save round info in files
            for (int i = 0; i < Rounds.Count; i++)
            {
                Rounds[i].Save(Hash + "_" + i.ToString() + ".xml");
            }
        }

        static public bool IsReplayExist(string Hash)
        {
            return File.Exists(GetFileName(Hash));
        }

        static private string GetFileName(string Hash)
        {
            return "replay/" + Hash + ".xml";
        }

        public void ReplayGame()
        {
            for (int i = 0; i < Rounds.Count; i++) Rounds[i].ReplayGame();
        }

        public bool LoadXml(string Hash)
        {
            string FileName = "replay/" + Hash + ".xml";
            XmlLoad X = new XmlLoad();

            if (!X.Load(FileName)) return false;
            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "hash": this.Hash = X.GetAttribute("value"); break;
                    case "playercount": PlayerCount = X.GetIntAttribute("value"); break;
                    case "lobby": PlayerCount = X.GetIntAttribute("lobby"); break;
                    case "lobbytype": PlayerCount = X.GetIntAttribute("lobbytype"); break;
                    case "balance": Balance = X.ReadIntArray(); break;
                    case "result": Result = X.ReadIntArray(); break;
                    case "place": Place = X.ReadIntArray(); break;
                    case "playerlist":
                        {
                            int Count = 4;
                            XmlLoad Subtree = X.GetSubtree();

                            for (int j = 0; j < Count; j++)
                            {
                                Players[j] = new Player();
                                Players[j].ReadXml(Subtree);
                            }
                        }
                        break;
                    case "roundlist":
                        {
                            int Count = X.GetIntAttribute("count");

                            XmlLoad Subtree = X.GetSubtree();
                            for (int j = 0; j < Count; j++)
                            {
                                if (!Subtree.Read()) break;

                                switch (Subtree.ElementName)
                                {
                                    case "round":
                                        string FName = Subtree.GetAttribute("filename");
                                        int Index = Subtree.GetIntAttribute("index");
                                        Round R = new Round();
                                        Rounds.Add(R);

                                        RoundFiles.Add(FName);

                                        XmlLoad RoundData = Subtree.GetSubtree();
                                        while (RoundData.Read())
                                        {
                                            switch (Subtree.ElementName)
                                            {
                                                case "round": R.CurrentRound = RoundData.GetIntAttribute("index"); break;
                                                case "renchan": R.RenchanStick = RoundData.GetIntAttribute("index"); break;
                                                case "result": R.StringResult = RoundData.GetAttribute("value"); break;
                                                case "balancebefore": R.BalanceBefore = RoundData.ReadIntArray(); break;
                                                case "balanceafter": R.BalanceAfter = RoundData.ReadIntArray(); break;
                                                case "pay": R.Pay = RoundData.ReadIntArray(); break;
                                                case "han": R.HanCount = RoundData.ReadIntArray(); break;
                                                case "fu": R.FuCount = RoundData.ReadIntArray(); break;
                                                case "cost": R.Cost = RoundData.ReadIntArray(); break;
                                                case "winner": R.Winner = RoundData.ReadBoolArray(); break;
                                                case "loser": R.Loser = RoundData.ReadBoolArray(); break;
                                                case "openedsets": R.OpenedSets = RoundData.ReadIntArray(); break;
                                                case "riichi": R.Riichi = RoundData.ReadIntArray(); break;
                                                case "dealer": R.Dealer = RoundData.ReadBoolArray(); break;
                                            }
                                        }

                                        // try to load more data
                                        R.Load(FName);
                                        break;
                                }
                            }
                        }
                        break;
                }
            }

            Date = GetDateTimeFromHash(Hash);

            return true;
        }

        private DateTime GetDateTimeFromHash(string Hash)
        {
            // yyyymmdd
            int Year = Convert.ToInt16(Hash.Substring(0, 4));
            int Month = Convert.ToInt16(Hash.Substring(4, 2));
            int Day = Convert.ToInt16(Hash.Substring(6, 2));
            int Hour = Convert.ToInt16(Hash.Substring(8, 2));

            DateTime Result = new DateTime(Year, Month, Day, Hour, 0, 0);

            return Result;
        }

        public void SaveXml(string FileName)
        {
            XmlSave X = new XmlSave(FileName);

            X.StartXML("mjreplay");

            // replay ID
            X.WriteTag("hash", "value", Hash);
            X.WriteTag("playercount", "value", PlayerCount);
            X.WriteTag("lobby", "value", Lobby);
            X.WriteTag("lobbytype", "value", LobbyType);
            X.WriteTag("result", Result);
            X.WriteTag("balance", Balance);
            X.WriteTag("place", Place);

            {
                X.StartTag("playerlist");

                for (int j = 0; j < 4; j++)
                {
                    Players[j].WriteXml(X);
                }
                X.EndTag();
            }

            {
                X.StartTag("roundlist");
                X.Attribute("count", Rounds.Count);
                for (int i = 0; i < Rounds.Count; i++)
                {
                    if(Rounds[i].FileName.CompareTo("") == 0) Rounds[i].FileName = Hash + "_" + i.ToString() + ".xml";

                    X.StartTag("round");
                    X.Attribute("index", i);
                    X.Attribute("filename", Rounds[i].FileName);

                    // Results
                    {
                        int[] StepCount = new int[4];
                        for (int j = 0; j < 4; j++) StepCount[j] = 0;
                        for (int j = 0; j < Rounds[i].Steps.Count; j++)
                        {
                            if (Rounds[i].Steps[j].Type == StepType.STEP_DISCARDTILE) StepCount[Rounds[i].Steps[j].Player]++;
                        }

                        X.WriteTag("round", "index", Rounds[i].CurrentRound);
                        X.WriteTag("renchan", "index", Rounds[i].RenchanStick);
                        X.WriteTag("result", "value", Rounds[i].StringResult);

                        X.WriteTag("balancebefore", Rounds[i].BalanceBefore);
                        X.WriteTag("balanceafter", Rounds[i].BalanceAfter);
                        X.WriteTag("pay", Rounds[i].Pay);
                        X.WriteTag("han", Rounds[i].HanCount);
                        X.WriteTag("fu", Rounds[i].FuCount);
                        X.WriteTag("cost", Rounds[i].Cost);
                        X.WriteTag("steps", StepCount);
                        X.WriteTag("winner", Rounds[i].Winner);
                        X.WriteTag("loser", Rounds[i].Loser);
                        X.WriteTag("openedsets", Rounds[i].OpenedSets);
                        X.WriteTag("riichi", Rounds[i].Riichi);
                        X.WriteTag("dealer", Rounds[i].Dealer);
                    }

                    X.EndTag();
                }
                X.EndTag();
            }

            X.EndXML();
            X.Close();
        }
    }
}

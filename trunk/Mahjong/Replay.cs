using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace TenhouViewer.Mahjong
{
    class Replay
    {
        public string Hash = "";
        public Player[] Players = new Player[4];
        public List<Round> Rounds = new List<Round>();
        public List<string> RoundFiles = new List<string>();

        public int[] Place = new int[4];

        public Replay()
        {

        }

        public void Save()
        {
            SaveXml("replay/" + Hash + ".xml");
            // Save round info in files
            for (int i = 0; i < Rounds.Count; i++)
            {
                Rounds[i].Save(Hash + "_" + i.ToString() + ".xml");
            }
        }

        public void LoadXml(string Hash)
        {
            string FileName = "replay/" + Hash + ".xml";
            XmlLoad X = new XmlLoad();

            if (!X.Load(FileName)) return;
            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "hash": Hash = X.GetAttribute("value"); break;
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
        }

        public void SaveXml(string FileName)
        {
            XmlSave X = new XmlSave(FileName);

            X.StartXML("mjreplay");

            // replay ID
            X.WriteTag("hash", "value", Hash);

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

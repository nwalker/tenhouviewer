using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace TenhouViewer.Mahjong
{
    enum RoundResult
    {
        Unknown,
        Draw,
        Ron,
        Tsumo
    }

    class Round
    {
        public string FileName = "";
        public string Hash = "";
        public int Index = 0;

        public Wall Wall = new Wall();
        public List<Step> Steps = new List<Step>();
        public Hand[] Hands = new Hand[4]; // Start hands

        public List<Yaku>[] Yaku = new List<Yaku>[4];

        public int[] Pay = new int[4];
        public int[] BalanceBefore = new int[4];
        public int[] BalanceAfter = new int[4];

        public int[] HanCount = new int[4];
        public int[] FuCount = new int[4];
        public int[] Cost = new int[4];

        public int[] OpenedSets = new int[4];
        public int[] Riichi = new int[4];

        public bool[] Dealer = new bool[4];
        public bool[] Winner = new bool[4];
        public bool[] Loser = new bool[4];

        public List<int> Dora = new List<int>();
        public List<int> UraDora = new List<int>();

        public RoundResult Result;

        public int CurrentRound = -1; // 0: e1, 1: e2, 2: e3, 3: e4, 4: s1, ...

        public int RenchanStick = 0;
        public int RiichiStick = 0;

        public Round()
        {
            Result = RoundResult.Unknown;

            for (int i = 0; i < 4; i++)
            {
                Yaku[i] = new List<Yaku>();

                Pay[i] = 0;
                HanCount[i] = 0;
                FuCount[i] = 0;
                Cost[i] = 0;
                Riichi[i] = -1;
                OpenedSets[i] = 0;

                Dealer[i] = false;
                Loser[i] = false;
                Winner[i] = false;

                BalanceBefore[i] = 0;
                BalanceAfter[i] = 0;
            }
        }

        public void Load(string FileName)
        {
            FileName = "round/" + FileName;

            XmlLoad X = new XmlLoad();

            if (!X.Load(FileName)) return;

            this.FileName = FileName;

            while (X.Read())
            {
                switch (X.ElementName)
                {
                    case "hash": Hash = X.GetAttribute("value"); break;
                    case "game": Index = X.GetIntAttribute("index"); break;
                    case "round": CurrentRound = X.GetIntAttribute("index"); break;
                    case "result": StringResult = X.GetAttribute("value"); break;
                    case "riichistick": RiichiStick = X.GetIntAttribute("value"); break;
                    case "renchanstick": RenchanStick = X.GetIntAttribute("value"); break;
                    case "balancebefore": BalanceBefore = X.ReadIntArray(); break;
                    case "balanceafter": BalanceAfter = X.ReadIntArray(); break;
                    case "pay": Pay = X.ReadIntArray(); break;
                    case "winner": Winner = X.ReadBoolArray(); break;
                    case "loser": Loser = X.ReadBoolArray(); break;
                    case "openedsets": OpenedSets = X.ReadIntArray(); break;
                    case "riichi": Riichi = X.ReadIntArray(); break;
                    case "dealer": Dealer = X.ReadBoolArray(); break;
                    case "steps":
                        {
                            int Count = X.GetIntAttribute("count");
                            XmlLoad Subtree = X.GetSubtree();
 
                            for (int j = 0; j < Count; j++)
                            {
                                Step NewStep = new Step(-1);
                                NewStep.ReadXml(Subtree);

                                Steps.Add(NewStep);
                            }
                        }
                        break;
                    case "agari":
                        {
                            int Player = X.GetIntAttribute("player");

                            HanCount[Player] = X.GetIntAttribute("han");
                            FuCount[Player] = X.GetIntAttribute("fu");
                            Cost[Player] = X.GetIntAttribute("cost");

                            XmlLoad Subtree = X.GetSubtree();
                            while (Subtree.Read())
                            {
                                switch (Subtree.ElementName)
                                {
                                    case "yakulist":
                                        int Count = Subtree.GetIntAttribute("count");
                                        {
                                            XmlLoad YakuList = Subtree.GetSubtree();

                                            while (YakuList.Read())
                                            {
                                                switch (YakuList.ElementName)
                                                {
                                                    case "yaku":
                                                        Yaku Y = new Yaku();
                                                        Y.Cost = YakuList.GetIntAttribute("cost");
                                                        Y.Index = YakuList.GetIntAttribute("index");

                                                        Yaku[Player].Add(Y);
                                                        break;
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                    case "dora":
                        {
                            int Count = X.GetIntAttribute("count");

                            XmlLoad Subtree = X.GetSubtree();
                            while (Subtree.Read())
                            {
                                switch (Subtree.ElementName)
                                {
                                    case "dora":
                                        Dora.Add(Subtree.GetIntAttribute("value"));
                                        break;
                                }
                            }
                        }
                        break;
                    case "uradora":
                        {
                            int Count = X.GetIntAttribute("count");

                            XmlLoad Subtree = X.GetSubtree();
                            while (Subtree.Read())
                            {
                                switch (Subtree.ElementName)
                                {
                                    case "dora":
                                        UraDora.Add(Subtree.GetIntAttribute("value"));
                                        break;
                                }
                            }
                        }
                        break;
                }
            }
        }

        public void Save(string FileName)
        {
            FileName = "round/" + FileName;

            XmlSave X = new XmlSave(FileName);

            this.FileName = FileName;

            X.StartXML("mjround");

            // Что это за раздача?
            X.WriteTag("hash", "value", Hash);
            X.WriteTag("game", "index", Index);
            X.WriteTag("round", "index", CurrentRound);
            X.WriteTag("result", "value", StringResult);
            X.WriteTag("riichistick", "value", RenchanStick);
            X.WriteTag("renchanstick", "value", RiichiStick);

            X.WriteTag("balancebefore", BalanceBefore);
            X.WriteTag("balanceafter", BalanceAfter);
            X.WriteTag("pay", Pay);
            X.WriteTag("winner", Winner);
            X.WriteTag("loser", Loser);
            X.WriteTag("openedsets", OpenedSets);
            X.WriteTag("riichi", Riichi);
            X.WriteTag("dealer", Dealer);

            // Actions
            {
                X.StartTag("steps");
                X.Attribute("count", Steps.Count);

                for (int j = 0; j < Steps.Count; j++)
                {
                    Steps[j].WriteXml(X);
                }

                X.EndTag();
            }

            // Yaku list
            {

                for (int i = 0; i < 4; i++)
                {
                    if (Yaku[i].Count == 0) continue;

                    X.StartTag("agari");
                    X.Attribute("player", i);
                    X.Attribute("han", HanCount[i]);
                    X.Attribute("fu", FuCount[i]);
                    X.Attribute("cost", Cost[i]);
                    {
                        X.StartTag("yakulist");
                        X.Attribute("count", Yaku[i].Count);
                        for (int j = 0; j < Yaku[i].Count; j++)
                        {
                            X.StartTag("yaku");

                            X.Attribute("index", Yaku[i][j].Index);
                            X.Attribute("cost", Yaku[i][j].Cost);

                            X.EndTag();
                        }
                        X.EndTag();
                    }
                    X.EndTag();
                }
            }

            // Dora
            {
                X.StartTag("dora");
                X.Attribute("count", Dora.Count);

                for (int j = 0; j < Dora.Count; j++)
                {
                    X.WriteTag("dora", "value", Dora[j]);
                }

                X.EndTag();
            }

            if(UraDora.Count > 0)
            {
                X.StartTag("uradora");
                X.Attribute("count", UraDora.Count);

                for (int j = 0; j < UraDora.Count; j++)
                {
                    X.WriteTag("dora", "value", UraDora[j]);
                }

                X.EndTag();
            }


            X.EndXML();
            X.Close();
        }

        public string StringResult
        {
            get
            {
                switch (Result)
                {
                    case RoundResult.Draw: return "draw";
                    case RoundResult.Ron: return "ron";
                    case RoundResult.Tsumo: return "tsumo";

                    default: return "unknown";
                }
            }

            set
            {
                switch (value)
                {
                    case "draw": Result = RoundResult.Draw; break;
                    case "ron": Result = RoundResult.Ron; break;
                    case "tsumo": Result = RoundResult.Tsumo; break;

                    default: Result = RoundResult.Unknown; break;
                }
            }
        }

    }
}

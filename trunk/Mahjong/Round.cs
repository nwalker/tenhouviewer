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

        public void Save(string FileName)
        {
            Xml X = new Xml(FileName);

            X.StartXML("mjround");

            // Что это за раздача?
            X.WriteTag("hash", "value", Hash);
            X.WriteTag("game", "index", Index);
            X.WriteTag("round", "index", CurrentRound);
            X.WriteTag("result", "value", StringResult);

            X.WriteTag("balancebefore", BalanceBefore);
            X.WriteTag("balanceafter", BalanceAfter);
            X.WriteTag("pay", Pay);
            X.WriteTag("winner", Winner);
            X.WriteTag("loser", Loser);
            X.WriteTag("openedsets", OpenedSets);
            X.WriteTag("riichi", Riichi);
            X.WriteTag("dealer", Dealer);

            X.WriteTag("riichistick", "value", RenchanStick);
            X.WriteTag("renchanstick", "value", RiichiStick);

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
        }

    }
}

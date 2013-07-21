using System;
using System.Collections.Generic;
using System.IO;
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
        public Hand[] StartHands = new Hand[4]; // Start hands
        public List<Hand>[] Hands = new List<Hand>[4];
        public List<int>[] Shanten = new List<int>[4];

        public List<int>[] WinWaiting = new List<int>[4];

        public List<Yaku>[] Yaku = new List<Yaku>[4];

        public int[] StepCount = new int[4];
        public int[] Pay = new int[4];
        public int[] BalanceBefore = new int[4];
        public int[] BalanceAfter = new int[4];

        public int[] HanCount = new int[4];
        public int[] FuCount = new int[4];
        public int[] Cost = new int[4];

        public int[] OpenedSets = new int[4];
        public int[] Riichi = new int[4];

        public bool[] Tempai = new bool[4];
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
                Hands[i] = new List<Hand>();
                Shanten[i] = new List<int>();
                WinWaiting[i] = new List<int>();

                Pay[i] = 0;
                HanCount[i] = 0;
                FuCount[i] = 0;
                Cost[i] = 0;
                Riichi[i] = -1;
                OpenedSets[i] = 0;
                StepCount[i] = 0;

                Tempai[i] = false;
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
                    case "tempai": Tempai = X.ReadBoolArray(); break;
                    case "stepcount": StepCount = X.ReadIntArray(); break;
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
                    case "hands":
                        {
                            XmlLoad Subtree = X.GetSubtree();

                            while (Subtree.Read())
                            {
                                switch (Subtree.ElementName)
                                {
                                    case "hand":
                                        {
                                            int Player = Subtree.GetIntAttribute("player");

                                            XmlLoad HandData = Subtree.GetSubtree();
                                            StartHands[Player] = new Hand();

                                            StartHands[Player].ReadXml(HandData);
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                    case "shantenlist":
                        {
                            XmlLoad Subtree = X.GetSubtree();

                            while (Subtree.Read())
                            {
                                switch (Subtree.ElementName)
                                {
                                    case "shanten":
                                        {
                                            int Player = Subtree.GetIntAttribute("player");
                                            int Count = Subtree.GetIntAttribute("shanten");

                                            XmlLoad ShantenData = Subtree.GetSubtree();

                                            while (ShantenData.Read())
                                            {
                                                switch (ShantenData.ElementName)
                                                {
                                                    case "step":
                                                        {
                                                            int Value = ShantenData.GetIntAttribute("value");
                                                            Shanten[Player].Add(Value);
                                                        }
                                                        break;
                                                }
                                            }
                                        }
                                        break;
                                }
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
                                        {
                                            int Count = Subtree.GetIntAttribute("count");
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
                                    case "waitings":
                                        {
                                            int Count = Subtree.GetIntAttribute("count");
                                            XmlLoad WaitList = Subtree.GetSubtree();

                                            while (WaitList.Read())
                                            {
                                                switch (WaitList.ElementName)
                                                {
                                                    case "waiting":
                                                        WinWaiting[Player].Add(WaitList.GetIntAttribute("value"));
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
            if (!Directory.Exists("round")) Directory.CreateDirectory("round");

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
            X.WriteTag("stepcount", StepCount);
            X.WriteTag("tempai", Tempai);

            // Start hands
            {
                X.StartTag("hands");
                for (int i = 0; i < StartHands.Length; i++)
                {
                    X.StartTag("hand");
                    X.Attribute("player", i);

                    StartHands[i].WriteXml(X);

                    X.EndTag();
                }

                X.EndTag();
            }

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

            // Shanten dynamic
            {
                X.StartTag("shantenlist");
                for (int i = 0; i < Shanten.Length; i++)
                {
                    if (Shanten[i].Count == 0) continue;

                    X.StartTag("shanten");
                    X.Attribute("player", i);
                    X.Attribute("count", Shanten[i].Count);

                    for (int j = 0; j < Shanten[i].Count; j++) X.WriteTag("step", "value", Shanten[i][j]);

                    X.EndTag();
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

                    if (Winner[i])
                    {
                        // Waiting hand
                        List<int> WaitingList = WinWaiting[i];

                        X.StartTag("waitings");
                        X.Attribute("count", WaitingList.Count);
                        for (int k = 0; k < WaitingList.Count; k++)
                        {
                            X.WriteTag("waiting", "value", WaitingList[k]);
                        }
                        X.EndTag();
                        break;
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

        // Get all hands in round
        public void ReplayGame()
        {
            Hand[] TempHands = new Hand[4];
            int LastTile = -1;

            // Init hands
            for (int i = 0; i < 4; i++)
            {
                Hands[i].Clear();
                Hands[i].Add(StartHands[i]);
                Shanten[i].Add(StartHands[i].Shanten);

                TempHands[i] = new Hand(StartHands[i]);
            }

            for (int i = 0; i < Steps.Count; i++)
            {
                Step S = Steps[i];

                switch (Steps[i].Type)
                {
                    case StepType.STEP_DRAWTILE:
                        {
                            TempHands[S.Player].Draw(S.Tile);
                        }
                        break;
                    case StepType.STEP_DRAWDEADTILE:
                        {
                            LastTile = S.Tile;

                            TempHands[S.Player].Draw(S.Tile);
                        }
                        break;
                    case StepType.STEP_DISCARDTILE:
                        {
                            TempHands[S.Player].Discard(S.Tile);

                            Hands[S.Player].Add(new Hand(TempHands[S.Player]));

                            int TShanten = TempHands[S.Player].Shanten;
                            if(TShanten == 0) Tempai[S.Player] = true;

                            Shanten[S.Player].Add(TShanten);
                        }
                        break;
                    case StepType.STEP_NAKI:
                        {
                            Hand H = TempHands[S.Player];

                            if (S.NakiData.Type == NakiType.CHAKAN)
                            {
                                Tile T = new Tile(S.NakiData.Tiles[0]);

                                // Remove pon with this tiles
                                for (int n = 0; n < H.Naki.Count; n++)
                                {
                                    if (new Tile(H.Naki[n].Tiles[0]).TileId == T.TileId)
                                    {
                                        H.Naki.RemoveAt(n);
                                    }
                                }
                            }

                            TempHands[S.Player].Naki.Add(S.NakiData);
                            TempHands[S.Player].OpenTiles(S.NakiData.Tiles);
                        }
                        break;
                    case StepType.STEP_TSUMO:
                        {
                            WinWaiting[S.Player] = Hands[S.Player][Hands[S.Player].Count - 1].WaitingList;

                            Hands[S.Player].Add(new Hand(TempHands[S.Player]));
                            Shanten[S.Player].Add(-1);
                        }
                        break;
                    case StepType.STEP_RON:
                        {
                            WinWaiting[S.Player] = Hands[S.Player][Hands[S.Player].Count - 1].WaitingList;

                            TempHands[S.Player].Draw(LastTile);

                            Hands[S.Player].Add(new Hand(TempHands[S.Player]));
                            Shanten[S.Player].Add(-1);
                        }
                        break;
                }
            }
        }
    }
}

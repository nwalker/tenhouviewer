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
                Rounds[i].Save("round/"+ Hash + "_" + i.ToString() + ".xml");
            }
        }

        public void SaveXml(string FileName)
        {
            Xml X = new Xml(FileName);

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
                    X.StartTag("round");
                    X.Attribute("index", i);
                    X.Attribute("filename", Hash + "_" + i.ToString() + ".xml");

                    // Results
                    {
                        int[] StepCount = new int[4];
                        for (int j = 0; j < 4; j++) StepCount[j] = 0;
                        for (int j = 0; j < Rounds[i].Steps.Count; j++)
                        {
                            if (Rounds[i].Steps[j].Type == StepType.STEP_DISCARDTILE) StepCount[Rounds[i].Steps[j].Player]++;
                        }

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

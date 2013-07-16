using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace TenhouViewer.Mahjong
{
    class Round
    {
        public string Hash = "";
        public int Index = 0;

        public Wall Wall = new Wall();
        public List<Step> Steps = new List<Step>();
        public Hand[] Hands = new Hand[4]; // Start hands

        public int[] Pay = new int[4];
        public int[] BalanceBefore = new int[4];
        public int[] BalanceAfter = new int[4];

        public Round()
        {
            
        }

        public void Save(string FileName)
        {
            Xml X = new Xml(FileName);

            X.StartXML("mjround");

            // Что это за раздача?
            X.WriteTag("hash", "value", Hash);
            X.WriteTag("game", "index", Index);

            X.WriteTag("balancebefore", BalanceBefore);
            X.WriteTag("balanceafter", BalanceAfter);
            X.WriteTag("pay", Pay);

            // Действия
            {
                X.StartTag("steps");
                X.Attribute("count", Steps.Count);

                for (int j = 0; j < Steps.Count; j++)
                {
                    Steps[j].WriteXml(X);
                }

                X.EndTag();
            }

            X.EndXML();
            X.Close();
        }
    }
}

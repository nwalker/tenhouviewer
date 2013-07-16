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

        public List<int>[] Yaku = new List<int>[4];

        public int[] Pay = new int[4];
        public int[] BalanceBefore = new int[4];
        public int[] BalanceAfter = new int[4];

        public int RenchanStick = 0;
        public int RiichiStick = 0;

        public Round()
        {
            for (int i = 0; i < 4; i++)
            {

            }
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

            X.WriteTag("riichistick", "value", RenchanStick);
            X.WriteTag("renchanstick", "value", RiichiStick);

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

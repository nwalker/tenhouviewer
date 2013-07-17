using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Mahjong
{
    public enum Sex
    {
        Unknown,
        Male,
        Female
    }

    class Player
    {
        public string NickName = "";
        public int Rank = 0;
        public int Rating = 0;
        public Sex Sex = Sex.Unknown;

        public Player()
        {

        }

        public void WriteXml(Xml X)
        {
            X.StartTag("player");
            X.Attribute("nick", NickName);
            X.Attribute("rank", Rank);
            X.Attribute("rating", Rating);

            string SexString = "";
            switch (Sex)
            {
                case Sex.Unknown: SexString = "U"; break;
                case Sex.Female: SexString = "F"; break;
                case Sex.Male: SexString = "M"; break;
            }

            X.Attribute("sex", SexString);

            X.EndTag();
        }
    }
}

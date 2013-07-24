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
        Female,
        Computer
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

        public void ReadXml(XmlLoad X)
        {
            if(!X.Read()) return;

            switch (X.ElementName)
            {
                case "player":
                    NickName = X.GetAttribute("nick");
                    Rank = X.GetIntAttribute("rank");
                    Rating = X.GetIntAttribute("rating");

                    string S = X.GetAttribute("sex");

                    switch (S)
                    {
                        case "M": Sex = Sex.Male; break;
                        case "F": Sex = Sex.Female; break;
                        case "C": Sex = Sex.Computer; break;

                        default: Sex = Sex.Unknown; break;
                    }
                    break;
            }
        }

        public void WriteXml(XmlSave X)
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
                case Sex.Computer: SexString = "C"; break;
            }

            X.Attribute("sex", SexString);

            X.EndTag();
        }
    }
}

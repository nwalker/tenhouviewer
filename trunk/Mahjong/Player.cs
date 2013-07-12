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
    }
}

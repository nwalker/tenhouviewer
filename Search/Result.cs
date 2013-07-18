using System;
using System.Collections.Generic;
using System.Text;

namespace TenhouViewer.Search
{
    class Result
    {
        public Mahjong.Replay Replay = null;

        // Is hand our result?

        public bool ReplayMark;
        public List<bool> RoundMark = new List<bool>();
        public bool[] PlayerMark = new bool[4];

        // In output use only HandMark field
        public List<bool[]> HandMark = new List<bool[]>();
    }
}

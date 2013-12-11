using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Tenhou
{
    static class Rank
    {
        readonly static string[] Ranks = { "新人", "9級", "8級", "7級", "6級", "5級", "4級", "3級", "2級", "1級", "初段", "二段", "三段", "四段", "五段", "六段", "七段", "八段", "九段", "十段" };

        public static string GetName(int Rank)
        {
            if ((Rank < 0) || (Rank >= Ranks.Length)) return "unk";
            
            return Ranks[Rank];
        }
    }
}

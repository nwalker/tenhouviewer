using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Tenhou
{
    static class Wind
    {
        public static string GetText(int Index)
        {
            switch (Index)
            {
                case 0: return "東";
                case 1: return "南";
                case 2: return "西";
                case 3: return "北";
            }

            return "";
        }
    }
}

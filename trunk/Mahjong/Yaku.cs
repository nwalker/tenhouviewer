using System;
using System.Collections.Generic;
using System.Text;

namespace TenhouViewer.Mahjong
{
    class Yaku
    {
        public int Index = -1;
        public int Cost = 0;

        public Yaku()
        {

        }

        public Yaku(int Index, int Cost)
        {
            this.Index = Index;
            this.Cost = Cost;
        }
    }
}

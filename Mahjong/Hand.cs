using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Mahjong
{
    class Hand
    {
        public int[] Tiles = new int[14];
        public int NakiCount = 0;

        public Hand()
        {

        }

        public void SetArray(int[] Tiles)
        {
            for (int i = 0; i < 13; i++) this.Tiles[i] = Tiles[i];
            this.Tiles[13] = -1;

            Array.Sort(this.Tiles);
        }
    }
}

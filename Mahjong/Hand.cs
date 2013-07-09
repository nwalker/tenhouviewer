using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Mahjong
{
    class Hand
    {
        int[] Tiles = new int[13];
        int DrowTile;

        public Hand()
        {

        }

        public void SetArray(int[] Tiles)
        {
            for (int i = 0; i < 13; i++) this.Tiles[i] = Tiles[i];

            Array.Sort(this.Tiles);
        }
    }
}

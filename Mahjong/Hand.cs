using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Mahjong
{
    class Hand
    {
        private int[] TileList = new int[13];

        public Hand()
        {

        }

        public void SetArray(int[] Tiles)
        {
            for (int i = 0; i < 13; i++) TileList[i] = Tiles[i];

            Array.Sort(TileList);
        }

        public int[] Tiles
        {
            get
            {
                int[] Temp = new int[13];
                for (int i = 0; i < 13; i++) Temp[i] = TileList[i];

                return Temp;
            }
        }
    }
}

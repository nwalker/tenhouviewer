using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace TenhouViewer.Paifu
{
    class PaifuTileImage
    {
        Mahjong.Tile Tile;
        public Bitmap Bmp;

        public PaifuTileImage(int Index, float Scale)
        {
            Tile = new Mahjong.Tile(Index);

            string TileName;

            switch(Index)
            {
                case -1: // closed tile
                    TileName = "0z";
                    break;
                case -2: // tsumogiri
                    TileName = "tsumogiri";
                    break;
                default:
                    TileName = Tile.TileName;
                    break;
            }

            string FileName = String.Format(".//tiles//{0:s}.gif", TileName);
            Bmp = new Bitmap(FileName);
            Bmp = new Bitmap(Bmp);
        }
    }
}

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

        public PaifuTileImage(int Index, float Scale, bool Red)
        {
            Tile = new Mahjong.Tile(Index);
            Tile.AkaNashi = !Red;

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

            // Add png option
            if (!File.Exists(FileName)) FileName = String.Format(".//tiles//{0:s}.png", TileName);

            Bitmap B = new Bitmap(FileName);
            Bmp = B;// new Bitmap(B);
        }
    }
}

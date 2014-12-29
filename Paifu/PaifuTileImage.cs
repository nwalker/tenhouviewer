using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace TenhouViewer.Paifu
{
    class PaifuTileImage
    {
        public static string TilesDirectory = "tiles";

        Mahjong.Tile Tile;
        public Bitmap Bmp;
        public Bitmap Original;

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

            string FileName = String.Format(".//{0:s}//{1:s}.gif", TilesDirectory, TileName);

            // Add png option
            if (!File.Exists(FileName)) FileName = String.Format(".//{0:s}//{1:s}.png", TilesDirectory, TileName);
            if (!File.Exists(FileName))
            {
                FileName = String.Format(".//tiles//{1:s}.gif", TilesDirectory, TileName);
                if (!File.Exists(FileName)) FileName = String.Format(".//tiles//{0:s}.png", TileName);
            }

            Bitmap B = new Bitmap(FileName);
            Original = B;// new Bitmap(B);
            Bmp = new Bitmap(Original);
        }

        public void Colorize(Color Target)
        {
            Bmp = new Bitmap(Original);

            using(Graphics G = Graphics.FromImage(Bmp))
            {
                Brush B = new SolidBrush(Color.FromArgb(50, Target));

                G.FillRectangle(B, new Rectangle(0, 0, Bmp.Width, Bmp.Height));
            }
        }
    }
}

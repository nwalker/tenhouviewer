using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TenhouViewer.Discarder
{
    class HandOutput
    {
        const float Scale = 0.7f;
        const int PaddingV = 20;
        const int PaddingH = 20;
        const int SpaceWidth = 5;

        // Calculated values
        int Width = 300;
        int Height = 300;
        int TileWidth = 20;
        int TileHeight = 30;
        
        Mahjong.Replay R;
        Mahjong.Round Rnd;
        bool Red;
        int Player;

        Bitmap B;
        Graphics G;

        public HandOutput(Mahjong.Replay Replay, int Round, int Player)
        {
            R = Replay;
            Rnd = R.Rounds[Round];
            this.Player = Player;

            Red = ((Replay.LobbyType & 0x0002) == 0x0002);
        }

        public void Generate()
        {
            CalcTileDimensions();

            B = new Bitmap(Width, Height);
            G = Graphics.FromImage(B);

            Replay();
        }

        private void CalcTileDimensions()
        {
            Paifu.PaifuTileImage Img = new Paifu.PaifuTileImage(-1, Scale, Red);

            TileWidth = Img.Bmp.Width;
            TileHeight = Img.Bmp.Height;

            Width = PaddingH * 2 + (TileWidth) * (28); // crop after drawing
            Height = 2 * PaddingV + TileHeight;
        }

        private int DrawTile(int Tile, int X, int Y, RotateFlipType Rotate)
        {
            Paifu.PaifuTileImage TileImage = new Paifu.PaifuTileImage(Tile, Scale, Red);
            Bitmap TileBitmap = TileImage.Bmp;

            switch (Rotate)
            {
                case RotateFlipType.Rotate90FlipNone: TileBitmap.RotateFlip(Rotate); Y += (TileHeight - TileWidth); break;
                case RotateFlipType.Rotate270FlipNone: TileBitmap.RotateFlip(Rotate); Y += (TileHeight - TileWidth); break;
            }

            G.DrawImage(TileBitmap, new Point(X, Y));

            return TileBitmap.Width;
        }

        private void Replay()
        {
            Rnd.ReplayGame();

            // Get last hand
            int HandIndex = Rnd.Hands[Player].Count - 1;
            Mahjong.Hand H = Rnd.Hands[Player][HandIndex];

            int tX = PaddingH;
            int tY = PaddingV;

            for (int i = 0; i < H.Tiles.Length; i++)
            {
                int Tile = H.Tiles[i];
                if (Tile == -1) continue;

                tX += DrawTile(Tile, tX, tY, RotateFlipType.RotateNoneFlipNone);
            }

            tX += SpaceWidth;

            for (int i = 0; i < H.Naki.Count; i++)
            {
                Mahjong.Naki N = H.Naki[H.Naki.Count - i - 1];

                switch (N.Type)
                {
                    case Mahjong.NakiType.NUKI:
                        tX += DrawTile(N.Tiles[0], tX, tY, RotateFlipType.RotateNoneFlipNone);
                        break;

                    case Mahjong.NakiType.CHI:
                        for (int j = 0; j < 3; j++)
                        {
                            RotateFlipType Rotate = (j == 0) ? RotateFlipType.Rotate90FlipNone : RotateFlipType.RotateNoneFlipNone;
                            tX += DrawTile(N.Tiles[j], tX, tY, Rotate);
                        }
                        break;
                    case Mahjong.NakiType.PON:
                        for (int j = 0; j < 3; j++)
                        {
                            RotateFlipType Rotate = (j == (3 - N.FromWho)) ? RotateFlipType.Rotate90FlipNone : RotateFlipType.RotateNoneFlipNone;

                            // 1: AB[C] 2: A[B]C 3: [A]BC
                            tX += DrawTile(N.Tiles[j], tX, tY, Rotate);
                        }
                        break;
                    case Mahjong.NakiType.ANKAN:
                        for (int j = 0; j < 4; j++)
                        {
                            int Tile = N.Tiles[j];

                            // Close first and last tiles
                            if ((j == 0) || (j == 3)) Tile = -1;
                            tX += DrawTile(Tile, tX, tY, RotateFlipType.RotateNoneFlipNone);
                        }
                        break;
                    case Mahjong.NakiType.MINKAN:
                        for (int j = 0; j < 4; j++)
                        {
                            RotateFlipType Rotate = RotateFlipType.RotateNoneFlipNone;

                            if (((N.FromWho == 1) && (j == 3)) ||
                                ((N.FromWho == 2) && (j == 1)) ||
                                ((N.FromWho == 3) && (j == 0))) Rotate = RotateFlipType.Rotate90FlipNone;

                            tX += DrawTile(N.Tiles[j], tX, tY, Rotate);
                        }
                        break;
                    case Mahjong.NakiType.CHAKAN:
                        for (int j = 0; j < 4; j++)
                        {
                            RotateFlipType Rotate = RotateFlipType.RotateNoneFlipNone;
                            int YOffset = 0;

                            if (j == (3 - N.FromWho))
                            {
                                Rotate = RotateFlipType.Rotate90FlipNone;
                            }
                            // Added tile
                            if (j == (4 - N.FromWho))
                            {
                                Rotate = RotateFlipType.Rotate90FlipNone;
                                tX -= TileHeight;
                                YOffset = -TileWidth;
                            }

                            tX += DrawTile(N.Tiles[j], tX, tY - YOffset, Rotate);
                        }
                        break;
                }

                tX += TileWidth / 2;
            }

            tX += PaddingH;
            B = B.Clone(new Rectangle(0, 0, tX, B.Height), System.Drawing.Imaging.PixelFormat.DontCare);
        }

        public void Save(string FileName)
        {
            B.Save(FileName);
        }
    }
}

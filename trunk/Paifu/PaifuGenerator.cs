using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TenhouViewer.Paifu
{
    class PaifuGenerator
    {
        readonly Font Fbig = new Font("Arial", 36.0f);
        readonly Font Fsmall = new Font("Arial", 12.0f);

        const float Scale = 0.7f;

        const int Width = 1000;
        int Height = 500;

        const int PaddingV = 10;
        const int PaddingH = 10;
        const int PlayerColumnWidth = 100;

        const int InternalPadding = 4;

        int InternalWidth;
        int InternalHeight;

        int FieldHeight;

        readonly string[] Winds = { "東", "南", "西", "北" };

        Mahjong.Replay R;
        Mahjong.Round Rnd;

        int[] Players = new int[4];
        int[] PlayerIndex = new int[4];
        int Dealer = 0;

        int Column = 0;

        int TileWidth = 0;
        int TileHeight = 0;
        
        Graphics G;

        public PaifuGenerator(Mahjong.Replay Replay, int Round)
        {
            R = Replay;
            Rnd = R.Rounds[Round];

            CalcPlayersPositions();
            CalcTileDimensions();

            Bitmap B = new Bitmap(Width, Height);
            G = Graphics.FromImage(B);

            DrawBorders();
            for (int i = 0; i < 4; i++) DrawHandInfo(i);
            DrawSteps();

            B.Save(Replay.Hash + "_" + Round.ToString() + ".png");
        }

        private void CalcTileDimensions()
        {
            PaifuTileImage Img = new PaifuTileImage(-1, Scale);

            TileWidth = Img.Bmp.Width;
            TileHeight = Img.Bmp.Height;


            Height = 2 * PaddingV + 4 * (2 * InternalPadding + 6 * TileHeight);
            InternalWidth = Width - 2 * PaddingH;
            InternalHeight = Height - 2 * PaddingV;

            FieldHeight = InternalHeight / 4;
        }

        private void CalcPlayersPositions()
        {
            // find dealer
            for(int i = 0; i < 4; i++)
            {
                if (Rnd.Dealer[i])
                {
                    Dealer = i;
                    break;
                }
            }

            // fill Players array:
            for (int i = 0; i < 4; i++)
            {
                Players[i] = (i + Dealer) & 0x03;
                PlayerIndex[i] = (4 - Dealer + i) & 0x03;
            }
        }

        private void DrawBorders()
        {
            Pen P = new Pen(Color.Black, 2.0f);

            // Paifu border
            G.DrawRectangle(P, new Rectangle(PaddingH, PaddingV, InternalWidth, InternalHeight));

            // Players border
            G.DrawRectangle(P, new Rectangle(PaddingH, PaddingV, PlayerColumnWidth, InternalHeight));

            // Draw horisontal lines
            {
                for (int i = 0; i < 4; i++) G.DrawLine(P, PaddingH, PaddingH + FieldHeight * i, PaddingH + InternalWidth, PaddingH + FieldHeight * i);
            }
        }

        private void DrawSteps()
        {
            int LastTile = -1;
            int LastPlayer = -1;

            for (int i = 0; i < Rnd.Steps.Count; i++)
            {
                Mahjong.Step S = Rnd.Steps[i];
                switch (S.Type)
                {
                    case Mahjong.StepType.STEP_DRAWTILE:
                        {
                            if (S.Player == Dealer) Column++;

                            // Is tsumogiri
                            bool Tsumogiri = ((Rnd.Steps[i + 1].Type == Mahjong.StepType.STEP_DISCARDTILE) &&
                                (Rnd.Steps[i + 1].Tile == S.Tile));

                            LastTile = S.Tile;
                            LastPlayer = PlayerIndex[S.Player];

                            DrawTsumoTile(PlayerIndex[S.Player], S.Tile, "", Tsumogiri);
                        }
                        break;
                    case Mahjong.StepType.STEP_DISCARDTILE:
                        {
                            LastTile = S.Tile;

                            DrawDiscardTile(PlayerIndex[S.Player], S.Tile, "");
                        }
                        break;
                    case Mahjong.StepType.STEP_NAKI:
                        {
                            // Need find draw or discard tile step
                            string NakiType = "unk";

                            switch (S.NakiData.Type)
                            {
                                case Mahjong.NakiType.CHI: NakiType = "chi"; break;
                                case Mahjong.NakiType.PON: NakiType = "pon"; break;
                                case Mahjong.NakiType.ANKAN: NakiType = "kan"; break;
                                case Mahjong.NakiType.MINKAN: NakiType = "kan"; break;
                                case Mahjong.NakiType.CHAKAN: NakiType = "kan"; break;
                            }

                            if (LastPlayer > PlayerIndex[S.Player]) Column++;
                            DrawTsumoTile(PlayerIndex[S.Player], LastTile, NakiType, false);
                        }
                        break;
                }
            }
        }

        private PointF DrawCenteredString(Font F, string S, PointF Pointer, int Width)
        {
            Brush Br = new SolidBrush(Color.Black);

            SizeF Size = G.MeasureString(S, F);
            float fX = Pointer.X + (Width - Size.Width) / 2;
            float fY = Pointer.Y + Size.Height / 10;

            // Draw wind indicator
            G.DrawString(S, F, Br, fX, fY);

            return new PointF(Pointer.X, fY + Size.Height);
        }

        private void DrawHandInfo(int Index)
        {
            int Player = Players[Index];

            float X = PaddingH;
            float Y = Index * FieldHeight + PaddingV;
            PointF Pointer = new PointF(X, Y);

            Pointer = DrawCenteredString(Fbig, Winds[Index], Pointer, PlayerColumnWidth);
            Pointer = DrawCenteredString(Fsmall, R.Players[Player].NickName, Pointer, PlayerColumnWidth);
            Pointer = DrawCenteredString(Fsmall, Rnd.BalanceBefore[Player].ToString(), Pointer, PlayerColumnWidth);
        }

        private void DrawTsumoTile(int Index, int Tile, string Comment, bool Tsumogiri)
        {
            int X = PaddingH + PlayerColumnWidth + InternalPadding + (Column + 1) * TileWidth;
            int Y = Index * FieldHeight + PaddingV + InternalPadding + (TileHeight * 2);

            if (Tsumogiri) Tile = -2;

            Bitmap TileBitmap = new PaifuTileImage(Tile, Scale).Bmp;

            G.DrawImage(TileBitmap, new Point(X, Y));

            DrawCenteredString(Fsmall, Comment, new PointF(X, Y - G.MeasureString(Comment, Fsmall).Height), TileWidth);
        }

        private void DrawDiscardTile(int Index, int Tile, string Comment)
        {
            int X = PaddingH + PlayerColumnWidth + InternalPadding + (Column + 1) * TileWidth;
            int Y = Index * FieldHeight + PaddingV + InternalPadding + (TileHeight * 3);

            Bitmap TileBitmap = new PaifuTileImage(Tile, Scale).Bmp;

            G.DrawImage(TileBitmap, new Point(X, Y));

            DrawCenteredString(Fsmall, Comment, new PointF(X, Y + TileHeight), TileWidth);
        }
    }
}

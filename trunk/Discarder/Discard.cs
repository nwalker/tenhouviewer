using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TenhouViewer.Discarder
{
    class Discard
    {
        const float Scale = 0.7f;
        const int PaddingV = 20;
        const int PaddingH = 20;

        // Calculated values
        int TileWidth = 20;
        int TileHeight = 30;
        int Width = 300;
        int Height = 300;

        int X = 0;
        int Y = 0;
        int Counter = 0;

        Mahjong.Replay R;
        Mahjong.Round Rnd;
        bool Red;
        int Player;

        public int HowManyDraw = -1;
        public bool RiichiLimit = false;
        public bool HighlightTsumogiri = false;
        public bool HighlightNaki = false;

        Bitmap B;
        Graphics G;

        Color NakiColor = Color.FromArgb(119, 119, 200, 0);
        Color GiriColor = Color.FromArgb(50, 50, 50, 0);

        public Discard(Mahjong.Replay Replay, int Round, int Player)
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

            int ExtraTiles = 0;

            TileWidth = Img.Bmp.Width;
            TileHeight = Img.Bmp.Height;

            int CircleCount = 0;

            for (int i = 0; i < Rnd.Steps.Count; i++)
            {
                if (Rnd.Steps[i].Player != Player) continue;

                if ((Rnd.Steps[i].Type == Mahjong.StepType.STEP_RIICHI) && RiichiLimit) break;
                if (Rnd.Steps[i].Type == Mahjong.StepType.STEP_DISCARDTILE) CircleCount++;
            }
            if(HowManyDraw != -1) CircleCount = Math.Min(CircleCount, HowManyDraw);

            if (CircleCount > 18)
            {
                ExtraTiles = CircleCount - 18;
            }
            else
            {
                if (Rnd.Riichi[Player] != 0) ExtraTiles += 1;
            }

            int Rows = ((CircleCount / 6) + 1);
            if (Rows > 3) Rows = 3;

            Width = PaddingH * 2 + (TileWidth) * (6 + ExtraTiles);
            Height = 2 * PaddingV + (TileHeight * Rows);
        }

        private bool IsTileNakied(int Step, int Tile)
        {
            for (int i = Step; i < Rnd.Steps.Count; i++)
            {
                Mahjong.Step S = Rnd.Steps[i];

                switch (S.Type)
                {
                    case Mahjong.StepType.STEP_DRAWTILE: return false;
                    case Mahjong.StepType.STEP_DRAWDEADTILE: return false;
                    case Mahjong.StepType.STEP_RON: return false;
                    case Mahjong.StepType.STEP_NAKI:
                        {
                            if(S.NakiData.Tiles.Contains(Tile)) return true;
                        }
                        break;
                }
            }

            return false;
        }

        private void DrawTile(int Tile, bool Riichi, bool Nakied, bool Tsumogiri)
        {
            Paifu.PaifuTileImage TileImage = new Paifu.PaifuTileImage(Tile, Scale, Red);
            if (Nakied && HighlightNaki)
                TileImage.Colorize(NakiColor);
            if (Tsumogiri && HighlightTsumogiri)
                TileImage.Colorize(GiriColor);
            Bitmap TileBitmap = TileImage.Bmp;

            if (Riichi)
                TileBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);

            int tX = PaddingH + X;
            int tY = PaddingV + Y;

            if (Riichi)
            {
                tY += TileBitmap.Width - TileBitmap.Height;
            }

            Counter++;
            X += TileBitmap.Width;
            if ((Counter == 6) || (Counter == 12))
            {
                Y += TileBitmap.Height;
                X = 0;
            }

            G.DrawImage(TileBitmap, new Point(tX, tY));
        }

        private void Replay()
        {
            bool RiichiFlag = false;
            bool Abort = false;
            int HowManyCounter = 0;
            int LastTile = -1;

            for (int i = 0; i < Rnd.Steps.Count; i++)
            {
                Mahjong.Step S = Rnd.Steps[i];
                if (S.Player != Player) continue;

                switch (S.Type)
                {
                    case Mahjong.StepType.STEP_DRAWDEADTILE:
                        LastTile = S.Tile;
                        break;
                    case Mahjong.StepType.STEP_DRAWTILE:
                        LastTile = S.Tile;
                        break;
                    case Mahjong.StepType.STEP_DISCARDTILE:
                        DrawTile(S.Tile, RiichiFlag, IsTileNakied(i, S.Tile), LastTile == S.Tile);
                        HowManyCounter++;
                        if (RiichiFlag && RiichiLimit) Abort = true;
                        RiichiFlag = false;
                        LastTile = -1;
                        break;
                    case Mahjong.StepType.STEP_RIICHI:
                        RiichiFlag = true;
                        break;
                }

                if (HowManyDraw != -1) if (HowManyDraw == HowManyCounter) break;
                if (Abort) break;
            }
        }

        public void Save(string FileName)
        {
            B.Save(FileName);
        }
    }
}

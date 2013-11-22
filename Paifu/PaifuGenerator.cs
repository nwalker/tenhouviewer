using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace TenhouViewer.Paifu
{
    class PaifuGenerator
    {
        readonly Font Fbig = new Font("Arial", 36.0f);
        readonly Font Fsmall = new Font("Arial", 12.0f);
        readonly Font Fcomment = new Font("Arial", 10.0f);

        const float Scale = 0.7f;

        int Width = 1100;
        int Height = 500;

        const int PaddingV = 10;
        const int PaddingH = 10;
        int PlayerColumnWidth = 100;
        int RoundColumnWidth = 100;
        int TilesColumnWidth = 1050;
        int YakuWidth = 180;
        int CostOffset = 75;
        int YakuOffset = 5;

        const int InternalPadding = 4;

        int InternalWidth;
        int InternalHeight;

        int FieldHeight;

        readonly string[] Winds = { "東", "南", "西", "北" };
        readonly string[] Ranks = { "新人", "9級", "8級", "7級", "6級", "5級", "4級", "3級", "2級", "1級", "初段", "二段", "三段", "四段", "五段", "六段", "七段", "八段", "九段", "十段" };

        Mahjong.Replay R;
        Mahjong.Round Rnd;

        int[] Players = new int[4];
        int[] PlayerIndex = new int[4];
        int Dealer = 0;

        int Column = 0;
        int LastTile = -1;

        int TileWidth = 0;
        int TileHeight = 0;

        Bitmap B;
        Graphics G;

        public PaifuGenerator(Mahjong.Replay Replay, int Round)
        {
            R = Replay;
            Rnd = R.Rounds[Round];
            // Replay only one game, if it need
            if (Rnd.Hands[0].Count == 0) Rnd.ReplayGame();

            CalcPlayersPositions();
            CalcTileDimensions();

            B = new Bitmap(Width, Height);
            G = Graphics.FromImage(B);

            DrawBorders();
            DrawRoundInfo();
            DrawSteps();
            for (int i = 0; i < R.PlayerCount; i++)
            {
                DrawHandInfo(i);
                DrawStartHand(i);
                DrawLastHand(i);
            }
        }

        public void Save(string FileName)
        {
            
            B.Save(FileName);
        }


        private void CalcTileDimensions()
        {
            PaifuTileImage Img = new PaifuTileImage(-1, Scale);

            TileWidth = Img.Bmp.Width;
            TileHeight = Img.Bmp.Height;

            int CircleCount = 23;
            {
                int StepCount = 12;

                for (int i = 0; i < Rnd.PlayerCount; i++) if (Rnd.StepCount[i] > StepCount) StepCount = Rnd.StepCount[i];

                CircleCount = StepCount + 4;
            }

            TilesColumnWidth = TileWidth * CircleCount + YakuWidth;

            Width = RoundColumnWidth + PlayerColumnWidth + TilesColumnWidth + PaddingH * 2;
            Height = 2 * PaddingV + R.PlayerCount * (2 * InternalPadding + 6 * TileHeight);

            InternalWidth = Width - 2 * PaddingH;
            InternalHeight = Height - 2 * PaddingV;

            RoundColumnWidth = TileWidth * 5;

            FieldHeight = InternalHeight / R.PlayerCount;
        }

        private void CalcPlayersPositions()
        {
            // find dealer
            for (int i = 0; i < R.PlayerCount; i++)
            {
                if (Rnd.Dealer[i])
                {
                    Dealer = i;
                    break;
                }
            }

            // fill Players array:
            for (int i = 0; i < R.PlayerCount; i++)
            {
                Players[i] = (i + Dealer) % R.PlayerCount;
                PlayerIndex[i] = (R.PlayerCount - Dealer + i) % R.PlayerCount;
            }
        }

        private void DrawBorders()
        {
            Pen P = new Pen(Color.Black, 2.0f);
            Brush Br = new SolidBrush(Color.White);

            // fill background
            G.FillRectangle(Br, new Rectangle(PaddingH, PaddingV, InternalWidth, InternalHeight));

            // Paifu border
            G.DrawRectangle(P, new Rectangle(PaddingH, PaddingV, InternalWidth, InternalHeight));

            // Round border
            G.DrawRectangle(P, new Rectangle(PaddingH, PaddingV, RoundColumnWidth, InternalHeight));

            // Players border
            G.DrawRectangle(P, new Rectangle(PaddingH + RoundColumnWidth, PaddingV, PlayerColumnWidth, InternalHeight));

            // Draw horisontal lines
            {
                for (int i = 0; i < 4; i++) G.DrawLine(P, PaddingH + RoundColumnWidth, PaddingV + FieldHeight * i, PaddingH + InternalWidth, PaddingV + FieldHeight * i);
            }
        }

        private void DrawRoundInfo()
        {
            List<int> DoraPointer = Rnd.GetDoraPointerList();
            List<int> UraDoraPointer = Rnd.GetUraDoraPointerList();

            int Wind = Rnd.CurrentRound / 4;
            int Index = (Rnd.CurrentRound & 3) + 1;

            string Round = String.Format("{0:s}{1:d}", Winds[Wind], Index);

            float X = PaddingH;
            float Y = PaddingV * 2;
            PointF Pointer = new PointF(X, Y);

            Pointer = DrawCenteredString(Color.Black, Fbig, Round, Pointer, RoundColumnWidth);
            Pointer = DrawCenteredString(Color.Black, Fcomment, R.Date.ToString("dd.MM.yyyy"), Pointer, RoundColumnWidth);

            Pointer.Y += PaddingV;
            int DoraY = Convert.ToInt32(Pointer.Y);
            Pointer.Y += TileHeight * 1.2f;

            int UraDoraY = Convert.ToInt32(Pointer.Y);

            // Ura
            for (int i = 0; i < 4; i++)
            {
                int Tile = (i < UraDoraPointer.Count) ? UraDoraPointer[i] : -1;

                DrawDoraTile(i, UraDoraY, Tile);
            }
            
            // Dora
            for (int i = 0; i < 4; i++)
            {
                int Tile = (i < DoraPointer.Count) ? DoraPointer[i] : -1;

                DrawDoraTile(i, DoraY, Tile);
            }


        }

        private void DrawHandInfo(int Index)
        {
            int Player = Players[Index];

            float X = PaddingH + RoundColumnWidth;
            float Y = Index * FieldHeight + PaddingV;
            PointF Pointer = new PointF(X, Y);

            string PlayerRank = String.Format("{0:s} {1:d}R", Ranks[R.Players[Player].Rank], R.Players[Player].Rating);

            Pointer = DrawCenteredString(Color.Black, Fbig, Winds[Index], Pointer, PlayerColumnWidth);
            Pointer = DrawCenteredString(Color.Black, Fsmall, R.Players[Player].NickName, Pointer, PlayerColumnWidth);
            Pointer = DrawCenteredString(Color.Black, Fsmall, PlayerRank, Pointer, PlayerColumnWidth);
            Pointer.Y += PaddingV;

            Pointer = DrawCenteredString(Color.Black, Fsmall, String.Format("{0:d}", Rnd.BalanceBefore[Player]), Pointer, PlayerColumnWidth);
            Pointer = DrawCenteredString(((Rnd.Pay[Player] >= 0) ? Color.Green : Color.Red), Fsmall, String.Format("{0:d}", Rnd.Pay[Player]), Pointer, PlayerColumnWidth);
        }

        private void DrawStartHand(int Index)
        {
            int Player = Players[Index];
            int Pos = 0;

            for (int i = 0; i < Rnd.StartHands[Player].Tiles.Length; i++)
            {
                int Tile = Rnd.StartHands[Player].Tiles[i];
                if(Tile == -1) continue;

                Pos = DrawHandTile(Index, Tile, Pos, 0, 0, RotateFlipType.RotateNoneFlipNone);
            }

            if(Rnd.Shanten[Player].Count > 0)
                DrawShanten(Index, 0, Convert.ToString(Rnd.Shanten[Player][0]));
        }

        private void DrawLastHand(int Index)
        {
            int Player = Players[Index];
            int Pos = 0;

            // Last hand
            Mahjong.Hand Hand = Rnd.Hands[Player][Rnd.Hands[Player].Count - 1];
            int[] Tiles = Hand.Tiles;

            for (int i = 0; i < Tiles.Length; i++)
            {
                int Tile = Tiles[i];
                if (Tile == -1) continue;
                if (Tile == LastTile) continue;

                Pos = DrawHandTile(Index, Tile, Pos, 5, 0, RotateFlipType.RotateNoneFlipNone);
            }
            Pos += TileWidth / 2;
            if (Rnd.Winner[Player])
            {
                Pos = DrawHandTile(Index, LastTile, Pos, 5, 0, RotateFlipType.RotateNoneFlipNone);
                Pos += TileWidth / 2;
            }

            if (Rnd.Shanten[Player].Count > 0)
            {
                int Shanten = Rnd.Shanten[Player][Rnd.Shanten[Player].Count - 1];
                DrawShanten(Index, 5, (Shanten >= 0) ? Shanten.ToString() : "アガリ");
            }

            for (int i = 0; i < Hand.Naki.Count; i++)
            {
                Mahjong.Naki N = Hand.Naki[i];

                switch (N.Type)
                {
                    case Mahjong.NakiType.NUKI:
                        Pos = DrawHandTile(Index, N.Tiles[0], Pos, 5, 0, RotateFlipType.RotateNoneFlipNone);
                        break;

                    case Mahjong.NakiType.CHI:
                        for (int j = 0; j < 3; j++)
                        {
                            RotateFlipType Rotate = (j == 0) ? RotateFlipType.Rotate90FlipNone : RotateFlipType.RotateNoneFlipNone;
                            Pos = DrawHandTile(Index, N.Tiles[j], Pos, 5, 0, Rotate);
                        }
                        break;
                    case Mahjong.NakiType.PON:
                        for (int j = 0; j < 3; j++)
                        {
                            RotateFlipType Rotate = (j == (3 - N.FromWho)) ? RotateFlipType.Rotate90FlipNone : RotateFlipType.RotateNoneFlipNone;

                            // 1: AB[C] 2: A[B]C 3: [A]BC
                            Pos = DrawHandTile(Index, N.Tiles[j], Pos, 5, 0, Rotate);
                        }
                        break;
                    case Mahjong.NakiType.ANKAN:
                        for (int j = 0; j < 4; j++)
                        {
                            int Tile = N.Tiles[j];

                            // Close first and last tiles
                            if((j == 0)||(j == 3)) Tile = -1;
                            Pos = DrawHandTile(Index, Tile, Pos, 5, 0, RotateFlipType.RotateNoneFlipNone);
                        }
                        break;
                    case Mahjong.NakiType.MINKAN:
                        for (int j = 0; j < 4; j++)
                        {
                            RotateFlipType Rotate = RotateFlipType.RotateNoneFlipNone;

                            if (((N.FromWho == 1) && (j == 3)) ||
                                ((N.FromWho == 2) && (j == 1)) ||
                                ((N.FromWho == 3) && (j == 0))) Rotate = RotateFlipType.Rotate90FlipNone;

                            Pos = DrawHandTile(Index, N.Tiles[j], Pos, 5, 0, Rotate);
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
                                Pos -= TileHeight;
                                YOffset = -TileWidth;
                            }

                            Pos = DrawHandTile(Index, N.Tiles[j], Pos, 5, YOffset, Rotate);
                        }
                        break;
                }

                Pos += TileWidth / 2;
            }
        }

        private void DrawSteps()
        {
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

                            bool Tsumo = ((Rnd.Steps[i + 1].Type == Mahjong.StepType.STEP_TSUMO) &&
                                (Rnd.Steps[i + 1].Player == S.Player));

                            LastTile = S.Tile;
                            LastPlayer = PlayerIndex[S.Player];

                            string Comment = (Tsumo) ? "ツモ" : "";
                            DrawTsumoTile(PlayerIndex[S.Player], S.Tile, Comment, Tsumogiri);

                            if (Tsumo) DrawYaku(PlayerIndex[S.Player], S.Player);
                        }
                        break;
                    case Mahjong.StepType.STEP_DRAWDEADTILE:
                        {
                            Column++;

                            bool Tsumogiri = ((Rnd.Steps[i + 1].Type == Mahjong.StepType.STEP_DISCARDTILE) &&
                                (Rnd.Steps[i + 1].Tile == S.Tile));

                            bool Tsumo = ((Rnd.Steps[i + 1].Type == Mahjong.StepType.STEP_TSUMO) &&
                                (Rnd.Steps[i + 1].Player == S.Player));

                            LastTile = S.Tile;
                            LastPlayer = PlayerIndex[S.Player];

                            string Comment = (Tsumo) ? "ツモ" : "";
                            DrawTsumoTile(PlayerIndex[S.Player], S.Tile, Comment, Tsumogiri);

                            if (Tsumo) DrawYaku(PlayerIndex[S.Player], S.Player);
                        }
                        break;
                    case Mahjong.StepType.STEP_DISCARDTILE:
                        {
                            LastTile = S.Tile;

                            // Need to find nearest riichi declaration step
                            bool Riichi = ((Rnd.Steps[i - 1].Type == Mahjong.StepType.STEP_RIICHI) &&
                                           (Rnd.Steps[i - 1].Player == S.Player));

                            bool Ron = ((Rnd.Steps[i + 1].Type == Mahjong.StepType.STEP_RON) &&
                                        (Rnd.Steps[i + 1].FromWho == S.Player));

                            string Comment = "";

                            if (Ron)
                                Comment = "ロン";
                            else if (Riichi)
                                Comment = "リーチ";

                            LastPlayer = PlayerIndex[S.Player];

                            DrawDiscardTile(PlayerIndex[S.Player], S.Tile, Comment);
                        }
                        break;
                    case Mahjong.StepType.STEP_NAKI:
                        {
                            // Need to find nearest draw or discard tile step
                            string NakiType = "unk";
                            bool Kan = false;

                            switch (S.NakiData.Type)
                            {
                                case Mahjong.NakiType.NUKI: NakiType = "抜き"; break;
                                case Mahjong.NakiType.CHI: NakiType = "チー"; break;
                                case Mahjong.NakiType.PON: NakiType = "ポン"; break;
                                case Mahjong.NakiType.ANKAN: NakiType = "カン"; Kan = true; break;
                                case Mahjong.NakiType.MINKAN: NakiType = "カン"; Kan = true; break;
                                case Mahjong.NakiType.CHAKAN: NakiType = "カン"; Kan = true; break;
                            }

                            if (LastPlayer > PlayerIndex[S.Player]) Column++;
                            LastPlayer = PlayerIndex[S.Player];

                            DrawTsumoTile(PlayerIndex[S.Player], LastTile, NakiType, false);

                            if (Kan && (S.NakiData.Type != Mahjong.NakiType.ANKAN)) Column++;
                            // Can be ron after chakan or ankan!
                        }
                        break;
                    case Mahjong.StepType.STEP_RON:
                        {
                            if ((LastPlayer > PlayerIndex[S.Player]) || (S.Player == Dealer)) Column++;

                            DrawRon(PlayerIndex[S.Player], "ロン", true);
                            DrawYaku(PlayerIndex[S.Player], S.Player);
                        }
                        break;
                }
            }
        }

        private PointF DrawCenteredString(Color C, Font F, string S, PointF Pointer, int Width)
        {
            Brush Br = new SolidBrush(C);

            SizeF Size = G.MeasureString(S, F);
            float fX = Pointer.X + (Width - Size.Width) / 2;
            float fY = Pointer.Y + Size.Height / 10;

            // Draw wind indicator
            G.DrawString(S, F, Br, fX, fY);

            return new PointF(Pointer.X, fY + Size.Height);
        }

        private int DrawHandTile(int Index, int Tile, int Pos, int Line, int YOffset, RotateFlipType Rotate)
        {
            Bitmap TileBitmap = new PaifuTileImage(Tile, Scale).Bmp;
            switch (Rotate)
            {
                case RotateFlipType.Rotate90FlipNone: TileBitmap.RotateFlip(Rotate); break;
                case RotateFlipType.Rotate270FlipNone: TileBitmap.RotateFlip(Rotate); break;
            }

            int X = PaddingH + RoundColumnWidth + PlayerColumnWidth + InternalPadding + Pos + TileWidth;
            int Y = Index * FieldHeight + PaddingV + InternalPadding + (TileHeight * Line) + YOffset + TileHeight - TileBitmap.Height;

            G.DrawImage(TileBitmap, new Point(X, Y));

            return Pos + TileBitmap.Width;
        }

        private void DrawTsumoTile(int Index, int Tile, string Comment, bool Tsumogiri)
        {
            int X = PaddingH + RoundColumnWidth + PlayerColumnWidth + InternalPadding + (Column) * TileWidth;
            int Y = Index * FieldHeight + PaddingV + InternalPadding + (TileHeight * 2);

            if (Tsumogiri) Tile = -2;

            Bitmap TileBitmap = new PaifuTileImage(Tile, Scale).Bmp;

            G.DrawImage(TileBitmap, new Point(X, Y));

            DrawCenteredString(Color.Black, Fcomment, Comment, new PointF(X, Y - G.MeasureString(Comment, Fcomment).Height), TileWidth);
        }

        private void DrawDiscardTile(int Index, int Tile, string Comment)
        {
            int X = PaddingH + RoundColumnWidth + PlayerColumnWidth + InternalPadding + (Column) * TileWidth;
            int Y = Index * FieldHeight + PaddingV + InternalPadding + (TileHeight * 3);

            Bitmap TileBitmap = new PaifuTileImage(Tile, Scale).Bmp;

            G.DrawImage(TileBitmap, new Point(X, Y));

            DrawCenteredString(Color.Black, Fcomment, Comment, new PointF(X, Y + TileHeight), TileWidth);
        }

        private void DrawDoraTile(int Index, int Y, int Tile)
        {
            int X = PaddingH + Index * TileWidth + TileWidth / 2;

            Bitmap TileBitmap = new PaifuTileImage(Tile, Scale).Bmp;

            G.DrawImage(TileBitmap, new Point(X, Y));
        }

        private void DrawRon(int Index, string Comment, bool Winner)
        {
            int Line = (Winner) ? 2 : 3; // draw tile line or discard tile line

            int X = PaddingH + RoundColumnWidth + PlayerColumnWidth + InternalPadding + (Column + 1 + ((!Winner) ? 1 : -1)) * TileWidth;
            int Y = Index * FieldHeight + PaddingV + InternalPadding + (TileHeight * Line);

            DrawCenteredString(Color.Black, Fcomment, Comment, new PointF(X, Y + TileHeight / 2 - G.MeasureString(Comment, Fcomment).Height / 2), TileWidth);
        }

        private void DrawYaku(int Index, int Player)
        {
            int X = PaddingH + RoundColumnWidth + PlayerColumnWidth + TilesColumnWidth - InternalPadding - YakuWidth;
            int Y = Index * FieldHeight + PaddingV + InternalPadding;

            Font F = Fcomment;
            Brush Br = new SolidBrush(Color.Black);

            for (int i = 0; i < Rnd.Yaku[Player].Count; i++)
            {
                int Yaku = Rnd.Yaku[Player][i].Index;
                int Cost = Rnd.Yaku[Player][i].Cost;

                string YakuName = Mahjong.YakuName.GetYakuName("jp", Yaku);

                G.DrawString(YakuName, F, Br, X, Y);
                G.DrawString(Cost.ToString(), F, Br, X + CostOffset, Y);

                Y += Convert.ToInt32(G.MeasureString(YakuName, F).Height + YakuOffset);
            }

            {
                string Fu = String.Format("{0:d}符", Rnd.FuCount[Player]);
                string Han = String.Format("{0:d}翻", Rnd.HanCount[Player]);

                G.DrawString(Fu, F, Br, X, Y);
                G.DrawString(Han, F, Br, X + CostOffset, Y);

                Y += Convert.ToInt32(G.MeasureString(Fu, F).Height + YakuOffset);
            }

            {
                string Cost = String.Format("{0:d}点", Rnd.Cost[Player]);
                G.DrawString(Cost, F, new SolidBrush(Color.Green), X, Y);
            }
        }

        private void DrawShanten(int Index, int Line, string Text)
        {
            int X = PaddingH + RoundColumnWidth + PlayerColumnWidth + InternalPadding;
            int Y = Index * FieldHeight + PaddingV + InternalPadding + (TileHeight * Line);

            DrawCenteredString(Color.Gray, Fcomment, Text, new PointF(X, Y + TileHeight / 2 - G.MeasureString(Text, Fcomment).Height / 2), TileWidth);
        }
    }
}

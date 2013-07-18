using System;
using System.Collections.Generic;
using System.Text;

namespace TenhouViewer.Mahjong
{
    class ShantenCalculator
    {
        private Hand Hand;
        private uint[] Tehai = new uint[38]; // Для анализа руки

        private int Mentsu; // Mentsu = Set
        private int Kouho;  // Kouho  = Incomplete set
        private int Toitsu; // Toitsu = Pair

        private int ShantenChiitoi; // Shanten to chiitoi hand (seven pairs)
        private int ShantenKokushi; // Shanten to kokushi hand (thirtheen orphans)
        private int ShantenNormal;  // Shanten to normal hand (four sets + pair)

        private int TempShantenNormal;

        private int Shanten;

        public ShantenCalculator(Hand Hand)
        {
            this.Hand = Hand;

            TehaiCreate(Hand);
        }

        private void TehaiCreate(Hand Hand)
        {
            int i;

            for (i = 0; i < 38; i++)
            {
                Tehai[i] = 0;
            }

            for (i = 0; i < 14; i++)
            {
                if (Hand.Tiles[i] != -1)
                {
                    Tile T = new Tile(Hand.Tiles[i]);

                    Tehai[T.TileId]++;
                }
            }
        }

        public int GetShanten()
        {
            ShantenNormal = CalcShantenNormal();
            ShantenChiitoi = CalcShantenChiitoi();
            ShantenKokushi = CalcShantenKokushi();
            
            Shanten = Math.Min(ShantenChiitoi, Math.Min(ShantenKokushi, ShantenNormal));

            return Shanten;
        }

        // Шантен до читойцу
        private int CalcShantenChiitoi()
        {
            int i;

            // In open hands chiitoi yaku not exist
            if (Hand.Naki.Count > 0) return 13;

            // Pair counting
            int PairCount = 0;
            for (i = 0; i < 38; i++) if (Tehai[i] >= 2) PairCount++;

            // Tile types counting
            int TypesCount = 0;
            for (i = 0; i < 38; i++) if (Tehai[i] > 0) TypesCount++;

            int TempShanten = 6 - PairCount;

            // 4 similar tiles - not 2 pair
            if (TypesCount < 7) TempShanten += 7 - TypesCount;

            return TempShanten;
        }

        private int CalcShantenKokushi()
        {
            int KokushiPair = 0;
            int TempShanten = 13;
            int i;

            if (Hand.Naki.Count  > 0) return 13;

            for (i = 1; i < 30; i++)
            {
                if ((i % 10 == 1) || (i % 10 == 9))
                {
                    if (Tehai[i] > 0) TempShanten--;
                    if ((Tehai[i] >= 2) && (KokushiPair == 0)) KokushiPair = 1;
                }
            }

            for (i = 31; i < 38; i++)
            {
                if (Tehai[i] > 0)
                {
                    TempShanten--;
                    if ((Tehai[i] >= 2) && (KokushiPair == 0)) KokushiPair = 1;
                }
            }
            // Если есть пара - уменьшим шантен
            TempShanten -= KokushiPair;

            return TempShanten;
        }

        private int CalcShantenNormal()
        {
            int i;

            // Maximal shanten in normal hand
            TempShantenNormal = 8;

            Mentsu = Hand.Naki.Count;
            Toitsu = 0;
            Kouho = 0;

            for (i = 1; i < 38; i++)
            {
                if (Tehai[i] >= 2)
                {
                    Toitsu++;
                    Tehai[i] -= 2;

                    CutMentsu(1);

                    Tehai[i] += 2;
                    Toitsu--;
                }
            }
            CutMentsu(1);

            return TempShantenNormal;
        }

        private void CutMentsu(int StartTile)
        {
            int i = StartTile;

            if (i < 38) for (; (i < 38) && (Tehai[i] == 0); i++) ;
            if (i >= 38) { CutTaatsu(1); return; }

            if (Tehai[i] >= 3)
            {
                Mentsu++;
                Tehai[i] -= 3;

                CutMentsu(i);

                Tehai[i] += 3;
                Mentsu--;
            }

            if ((i < 30) && (Tehai[i + 1] > 0) && (Tehai[i + 2] > 0))
            {
                Mentsu++;
                Tehai[i]--;
                Tehai[i + 1]--;
                Tehai[i + 2]--;

                CutMentsu(i);

                Tehai[i]++;
                Tehai[i + 1]++;
                Tehai[i + 2]++;
                Mentsu--;
            }

            CutMentsu(i + 1);
        }

        private void CutTaatsu(int Start)
        {
            int i = Start;

            if (i < 38) for (; (i < 38) && (Tehai[i] == 0); i++) ;
            if (i >= 38)
            {
                int Temp = 8 - Mentsu * 2 - Kouho - Toitsu;
                if (Temp <= TempShantenNormal)
                {
                    TempShantenNormal = Temp;
                }
                return;
            }

            if (Mentsu + Kouho < 4)
            {
                // Pairs
                if (Tehai[i] == 2)
                {
                    Kouho++;
                    Tehai[i] -= 2;

                    CutTaatsu(i);

                    Tehai[i] += 2;
                    Kouho--;
                }

                // Penchan and ryanmen form
                if ((i < 30) && (Tehai[i + 1] > 0))
                {
                    Kouho++;
                    Tehai[i]--; Tehai[i + 1]--;

                    CutTaatsu(i);

                    Tehai[i]++; Tehai[i + 1]++;
                    Kouho--;
                }

                // Kanchan form
                if ((i < 30) && ((i % 10) <= 8) && (Tehai[i + 2] > 0))
                {
                    Kouho++;
                    Tehai[i]--; Tehai[i + 2]--;

                    CutTaatsu(i);

                    Tehai[i]++; Tehai[i + 2]++;
                    Kouho--;
                }
            }
            CutTaatsu(i + 1);
        }
    }
}

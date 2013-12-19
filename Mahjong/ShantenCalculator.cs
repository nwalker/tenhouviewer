using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Mahjong
{
    enum FormType
    {
        Toitsu,  // Pair;
        Ankou,   // 3 similar tiles;
        Mentsu,  // 3 run tiles (1-2-3);
        Kanchan, // closed wait form : 1-3, 2-4, 3-5;
        Ryanmen, // opened wait form and end wait form: 12-, -23-, -34-, -45- и т.д.
        ToitsuWait, // Pair (to ankou);
    }

    class ShantenCalculator
    {
        private Hand Hand;
        private uint[] Tehai = new uint[38]; // Для анализа руки
        private uint[] BasicTehai = new uint[38]; // Неизменённая версия для анализа

        private int WaitingCount = -1; // количество сторон в выигрышном ожидании
        private int[] Waitings = new int[38]; // Выигрышные ожидания

        // Completed and uncompleted forms in hand
        private Stack Forms = new Stack();

        private int Mentsu; // Mentsu = Set
        private int Kouho;  // Kouho  = Incomplete set
        private int Toitsu; // Toitsu = Pair

        private int ShantenChiitoi; // Shanten to chiitoi hand (seven pairs)
        private int ShantenKokushi; // Shanten to kokushi hand (thirtheen orphans)
        private int ShantenNormal;  // Shanten to normal hand (four sets + pair)

        private int TempShantenNormal;

        private int Shanten = -1;

        public bool CompletedHand = false;

        public ShantenCalculator(Hand Hand)
        {
            this.Hand = Hand;

            TehaiCreate(Hand);
        }

        private void TehaiCreate(Hand Hand)
        {
            int i;

            WaitingCount = 0;

            for (i = 0; i < 38; i++)
            {
                Waitings[i] = 0;
                Tehai[i] = 0;
                BasicTehai[i] = 0;
            }

            for (i = 0; i < 14; i++)
            {
                if (Hand.Tiles[i] != -1)
                {
                    Tile T = new Tile(Hand.Tiles[i]);

                    Tehai[T.TileId]++;
                    BasicTehai[T.TileId]++;
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

        public List<int> WaitingList()
        {
            List<int> TempList = new List<int>();

            for (int i = 0; i < Waitings.Length; i++)
            {
                if (Waitings[i] > 0) TempList.Add(i);
            }

            return TempList;
        }

        private void CalcWaitings()
        {
            int i;

            Stack Temp = (Stack)Forms.Clone();
            bool IsSyanpon = false;
            int SyanponAlternativeWait = -1;

            for (i = 0; i < Forms.Count; i++)
            {
                uint Form = Convert.ToUInt32(Temp.Pop());

                // Decode form info
                FormType FormType = (FormType)((Form >> 24) & 0xFF);
                int Tile1 = Convert.ToInt32((Form >> 16) & 0xFF);
                int Tile2 = Convert.ToInt32((Form >> 8) & 0xFF);
                int Tile3 = Convert.ToInt32((Form >> 0) & 0xFF);

                switch (FormType)
                {
                    case FormType.Toitsu:
                        if (!IsSyanpon) break;

                        if (SyanponAlternativeWait == Tile1)
                        {
                            // Wait on similar tile?
                            if (Waitings[Tile1] > 0) Waitings[Tile1] = 0;
                        }
                        else
                        {
                            // Wait on different tile
                            if (Waitings[Tile1] == 0) { Waitings[Tile1] = 1; WaitingCount++; }
                        }
                        break;
                    case FormType.ToitsuWait:
                        IsSyanpon = true;
                        SyanponAlternativeWait = Tile1;
                        if (Waitings[Tile1] == 0) { Waitings[Tile1] = 1; WaitingCount++; }
                        break;
                    case FormType.Ryanmen:
                        // Wait from lower numbers
                        if (Tile1 % 10 != 1)
                        {
                            if (Waitings[Tile1 - 1] == 0) { Waitings[Tile1 - 1] = 1; WaitingCount++; }
                        }
                        // Wait from higher numbers
                        if (Tile2 % 10 != 9)
                        {
                            if (Waitings[Tile2 + 1] == 0) { Waitings[Tile2 + 1] = 1; WaitingCount++; }
                        }
                        break;
                    case FormType.Kanchan:
                        if (Waitings[Tile1 + 1] == 0) { Waitings[Tile1 + 1] = 1; WaitingCount++; }
                        break;
                }
            }
        }

        // Shanten to chiitoi hand
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

            // Waiting calculation
            if (TempShanten == 0)
            {
                for (i = 0; i < 38; i++)
                {
                    if ((Tehai[i] == 1)&&(Waitings[i] == 0)) { Waitings[i] = 1; WaitingCount++; }
                }
            }

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
            TempShanten -= KokushiPair;

            // Waiting calculation
            if (TempShanten == 0)
            {
                // Terminals?
                for (i = 1; i < 30; i++)
                {
                    if ((i % 10 == 1) || (i % 10 == 9))
                    {
                        if (Tehai[i] == 0)
                        {
                            if (Waitings[i] == 0) { Waitings[i] = 1; WaitingCount++; }
                        }
                    }
                }

                // Jihai?
                for (i = 31; i < 38; i++)
                {
                    if (Tehai[i] == 0)
                    {
                        if (Waitings[i] == 0) { Waitings[i] = 1; WaitingCount++; }
                    }
                }
            }

            return TempShanten;
        }

        private uint EncodeForm(FormType Type, int Tile1, int Tile2, int Tile3)
        {
            return (Convert.ToUInt32(Type) << 24) | (((uint)Tile1 & 0xFF) << 16) | (((uint)Tile2 & 0xFF) << 8) | ((uint)Tile3 & 0xFF);
        }

        private int CalcShantenNormal()
        {
            int i;

            // Maximal shanten in normal hand is 8
            TempShantenNormal = 8;
            CompletedHand = false;

            // Clear forms list
            Forms.Clear();

            Mentsu = 0;
            Toitsu = 0;
            Kouho = 0;

            // Count valuable naki
            for (i = 0; i < Hand.Naki.Count; i++)
            {
                if (Hand.Naki[i].Type != NakiType.NUKI) Mentsu++;
            }

            for (i = 1; i < 38; i++)
            {
                if (Tehai[i] >= 2)
                {
                    Toitsu++;
                    Tehai[i] -= 2;

                    Forms.Push(EncodeForm(FormType.Toitsu, i, i, 0));
                    CutMentsu(1);
                    Forms.Pop();

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

                Forms.Push(EncodeForm(FormType.Ankou, i, i, i));
                CutMentsu(i);
                Forms.Pop();

                Tehai[i] += 3;
                Mentsu--;
            }

            if ((i < 30) && (Tehai[i + 1] > 0) && (Tehai[i + 2] > 0))
            {
                Mentsu++;
                Tehai[i]--;
                Tehai[i + 1]--;
                Tehai[i + 2]--;

                Forms.Push(EncodeForm(FormType.Mentsu, i, i + 1, i + 2));
                CutMentsu(i);
                Forms.Pop();

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
                    if (Temp == 0)
                    {
                        if ((Kouho == 0) && (Toitsu == 0))
                        {
                            // Ожидание танки!
                            for (int j = 0; j < Tehai.Length; j++)
                            {
                                // Cannot wait on fifth tile!
                                if ((Tehai[j] > 0) && (BasicTehai[j] != 4))
                                {
                                    if (Waitings[j] == 0) { Waitings[j] = 1; WaitingCount++; }
                                }
                            }
                        }
                        if ((Kouho == 0) && (Toitsu == 1))
                        {
                            // Agari! Hand completed
                            CompletedHand = true;
                        }
                        else
                        {
                            // Tempai! Find all waitings in hand
                            CalcWaitings();
                        }
                    }

                    // If no actual waitings and tempai - ishanten!
                    if (!Waitings.Contains(1) && (Temp == 0)) Temp = 1;
                    
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

                    Forms.Push(EncodeForm(FormType.ToitsuWait, i, i, 0));
                    CutTaatsu(i);
                    Forms.Pop();


                    Tehai[i] += 2;
                    Kouho--;
                }

                // Penchan and ryanmen form
                if ((i < 30) && (Tehai[i + 1] > 0))
                {
                    Kouho++;
                    Tehai[i]--; Tehai[i + 1]--;

                    Forms.Push(EncodeForm(FormType.Ryanmen, i, i + 1, 0));
                    CutTaatsu(i);
                    Forms.Pop();

                    Tehai[i]++; Tehai[i + 1]++;
                    Kouho--;
                }

                // Kanchan form
                if ((i < 30) && ((i % 10) <= 8) && (Tehai[i + 2] > 0))
                {
                    Kouho++;
                    Tehai[i]--; Tehai[i + 2]--;

                    Forms.Push(EncodeForm(FormType.Kanchan, i, i + 2, 0));
                    CutTaatsu(i);
                    Forms.Pop();

                    Tehai[i]++; Tehai[i + 2]++;
                    Kouho--;
                }
            }
            CutTaatsu(i + 1);
        }
    }
}

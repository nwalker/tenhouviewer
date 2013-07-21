using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Search
{
    class GameFinder
    {
        private List<Result> GameList = new List<Result>();

        // Find queries
        // Fields use if they has not default values

        // NickName
        public string NickName = null;

        // Yaku (hand contain all yaku from this array)
        public int[] YakuList = null;

        // Waitings (hand has at least one tile in waiting from this array)
        public int[] Waitings = null;

        // Player place
        public int Place = 0;

        // Rank (1 = 1ku, 10 = 1dan, 11 = 2dan, ...)
        public int Rank = 0;

        // Is player dealer?
        public bool Dealer = false;

        // Is player winner?
        public bool Winner = false;

        // Is player loser (discard to ron)
        public bool Loser = false;

        // Maximum steps (tile discard) in round
        public int StepsMax = 0;

        // Amount of tiles to wait
        public int WaitingCountMin = -1;
        public int WaitingCountMax = -1;

        // Payment
        public int PaymentMin = -1;
        public int PaymentMax = -1;

        // Player rating
        public int RatingMin = -1;
        public int RatingMax = -1;

        // Shanten of start hand
        public int ShantenMax = -1;
        public int ShantenMin = -1;

        public GameFinder(Tenhou.TenhouHashList Hashes)
        {
            // Create blank ResultList from hash table
            foreach (string Hash in Hashes.Hashes)
            {
                Mahjong.Replay R = new Mahjong.Replay();

                if (R.LoadXml(Hash))
                {
                    Result Res = new Result();
                    Res.Replay = R;
                    Res.ReplayMark = true;

                    for (int i = 0; i < 4; i++) Res.PlayerMark[i] = true;

                    for (int i = 0; i < R.Rounds.Count; i++)
                    {
                        bool[] Marks = new bool[4];
                        for(int j = 0; j < 4; j++) Marks[j] = true;

                        Res.RoundMark.Add(true);
                        Res.HandMark.Add(Marks);
                    }

                    GameList.Add(Res);
                }
                else
                {
                    Console.WriteLine(Hash + " - not found");
                }
            }
        }

        // Finder can use results of previous queries
        public GameFinder(List<Result> Results)
        {
            // Reset marks 
            foreach (Result R in Results)
            {
                R.ReplayMark = true;

                for (int i = 0; i < R.RoundMark.Count; i++)
                {
                    //R.RoundMark[i] = true;
                    for (int j = 0; j < 4; j++) R.HandMark[i][j] = true;
                }

                for (int i = 0; i < 4; i++) R.PlayerMark[i] = true;

                GameList.Add(R);
            }
        }

        public List<Result> Find()
        {
            List<Result> ResultList = new List<Result>();

            for (int i = 0; i < GameList.Count; i++)
            {
                Result R = GameList[i];

                // filters
                CheckNickName(R);
                CheckYaku(R);
                CheckPlace(R);
                CheckRank(R);
                CheckPayment(R);
                CheckWinner(R);
                CheckLoser(R);
                CheckRating(R);
                CheckShanten(R);
                CheckSteps(R);
                CheckWaitings(R);

                // Check mark
                EmbedMarksToHandMark(R);
                if (!IsQueryOk(R)) continue;

                ResultList.Add(R);
            }

            return ResultList;
        }

        private bool IsQueryOk(Result R)
        {
            if (!R.ReplayMark) return false;
            if (!R.PlayerMark.Contains(true)) return false;

            for (int i = 0; i < R.RoundMark.Count; i++)
            {
                // If any round contain positive result...
                if (R.RoundMark[i]) return true;
            }

            return false;
        }

        private void EmbedMarksToHandMark(Result R)
        {
            for (int i = 0; i < R.RoundMark.Count; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (!R.PlayerMark[j]) R.HandMark[i][j] = false;
                    if (!R.RoundMark[i]) R.HandMark[i][j] = false;
                }

                // Exclude rounds which has no suitable hands
                R.RoundMark[i] = (R.RoundMark[i] && R.HandMark[i].Contains(true));
            }
        }

        // filters
        private void CheckNickName(Result R)
        {
            if (NickName == null) return;

            for (int i = 0; i < 4; i++)
            {
                if (R.Replay.Players[i].NickName.CompareTo(NickName) != 0)
                    R.PlayerMark[i] = false;
            }
        }

        private void CheckPlace(Result R)
        {
            if (Place == 0) return;

            for (int i = 0; i < 4; i++)
            {
                if (R.Replay.Place[i] == Place)
                    R.PlayerMark[i] = false;
            }
        }

        private void CheckRank(Result R)
        {
            if (Rank == 0) return;

            for (int i = 0; i < 4; i++)
            {
                if (R.Replay.Players[i].Rank != Rank)
                    R.PlayerMark[i] = false;
            }
        }

        private void CheckRating(Result R)
        {
            for (int i = 0; i < 4; i++)
            {
                if (RatingMin != -1)
                {
                    if (R.Replay.Players[i].Rating < RatingMin) R.PlayerMark[i] = false;
                }
                if (RatingMax != -1)
                {
                    if (R.Replay.Players[i].Rating > RatingMax) R.PlayerMark[i] = false;
                }
            }
        }

        private void CheckShanten(Result R)
        {
            for (int i = 0; i < R.Replay.Rounds.Count; i++)
            {
                Mahjong.Round Rnd = R.Replay.Rounds[i];

                for (int j = 0; j < 4; j++)
                {
                    if (Rnd.Shanten[j].Count > 0)
                    {
                        if (ShantenMin != -1) if (Rnd.Shanten[j][0] < ShantenMin) R.HandMark[i][j] = false;
                        if (ShantenMax != -1) if (Rnd.Shanten[j][0] > ShantenMax) R.HandMark[i][j] = false;
                    }
                    else
                    {
                        R.HandMark[i][j] = false;
                    }
                }
            }
        }

        private void CheckPayment(Result R)
        {
            for (int i = 0; i < R.Replay.Rounds.Count; i++)
            {
                Mahjong.Round Rnd = R.Replay.Rounds[i];

                for (int j = 0; j < 4; j++)
                {
                    if (PaymentMin != -1)
                    {
                        if (Rnd.Pay[j] < PaymentMin) R.HandMark[i][j] = false;
                    }
                    if (PaymentMax != -1)
                    {
                        if (Rnd.Pay[j] > PaymentMax) R.HandMark[i][j] = false;
                    }
                }
            }
        }

        private void CheckWinner(Result R)
        {
            if (!Winner) return;
            for (int i = 0; i < R.Replay.Rounds.Count; i++)
            {
                Mahjong.Round Rnd = R.Replay.Rounds[i];

                for (int j = 0; j < 4; j++)
                {
                    if (!Rnd.Winner[j]) R.HandMark[i][j] = false;
                }
            }
        }

        private void CheckLoser(Result R)
        {
            if (!Loser) return;
            for (int i = 0; i < R.Replay.Rounds.Count; i++)
            {
                Mahjong.Round Rnd = R.Replay.Rounds[i];

                for (int j = 0; j < 4; j++)
                {
                    if (!Rnd.Loser[j]) R.HandMark[i][j] = false;
                }
            }
        }

        private void CheckSteps(Result R)
        {
            if (StepsMax == 0) return;

            for (int i = 0; i < R.Replay.Rounds.Count; i++)
            {
                Mahjong.Round Rnd = R.Replay.Rounds[i];

                for (int j = 0; j < 4; j++)
                {
                    if (Rnd.StepCount[j] > StepsMax) R.HandMark[i][j] = false;
                }
            }
        }

        private void CheckWaitings(Result R)
        {
            if (!((WaitingCountMin == -1) && (WaitingCountMax == -1)))
            {

                for (int i = 0; i < R.Replay.Rounds.Count; i++)
                {
                    Mahjong.Round Rnd = R.Replay.Rounds[i];

                    for (int j = 0; j < 4; j++)
                    {
                        if (Rnd.WinWaiting[j].Count > WaitingCountMax) R.HandMark[i][j] = false;
                        if (Rnd.WinWaiting[j].Count < WaitingCountMin) R.HandMark[i][j] = false;
                    }
                }
            }

            if (Waitings != null)
            {
                for (int i = 0; i < R.Replay.Rounds.Count; i++)
                {
                    Mahjong.Round Rnd = R.Replay.Rounds[i];

                    for (int j = 0; j < 4; j++)
                    {
                        bool HasWait = false;
                        for (int k = 0; k < Rnd.WinWaiting[j].Count; k++)
                        {
                            if(Waitings.Contains(Rnd.WinWaiting[j][k]))
                            {
                                HasWait = true;
                                break;
                            }
                        }

                        if (!HasWait) R.HandMark[i][j] = false;
                    }
                }
            }
        }

        private bool IsYakuInYakuList(List<Mahjong.Yaku> YakuList, int Yaku)
        {
            for (int i = 0; i < YakuList.Count; i++)
            {
                if(YakuList[i].Index == Yaku) return true;
            }

            return false;
        }

        private void CheckYaku(Result R)
        {
            if (YakuList == null) return;

            for (int RoundIndex = 0; RoundIndex < R.Replay.Rounds.Count; RoundIndex++ )
            {
                Mahjong.Round Rnd = R.Replay.Rounds[RoundIndex];
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < YakuList.Length; j++)
                    {
                        if (!IsYakuInYakuList(Rnd.Yaku[i], YakuList[j]))
                        {
                            R.HandMark[RoundIndex][i] = false;
                            break;
                        }
                    }
                }
            }
        }

    }
}

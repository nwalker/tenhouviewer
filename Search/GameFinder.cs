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

        // Player count
        public int PlayerCount = 0;

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

        // Is round end in draw
        public bool Draw = false;
        public int DrawReason = -1;

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

        // Han count of winned hand
        public int HanMax = -1;
        public int HanMin = -1;

        // Fu count of winned hand
        public int FuMax = -1;
        public int FuMin = -1;

        // Lobby
        public int Lobby = -1;

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
                CheckPlayerCount(R);
                CheckNickName(R);
                CheckYaku(R);
                CheckHan(R);
                CheckFu(R);
                CheckPlace(R);
                CheckRank(R);
                CheckPayment(R);
                CheckDealer(R);
                CheckWinner(R);
                CheckLoser(R);
                CheckRating(R);
                CheckShanten(R);
                CheckSteps(R);
                CheckWaitings(R);
                CheckDraw(R);
                CheckLobby(R);

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

        private void CheckPlayerCount(Result R)
        {
            if (PlayerCount == 0) return;

            for (int i = 0; i < R.Replay.Rounds.Count; i++)
            {
                Mahjong.Round Rnd = R.Replay.Rounds[i];

                if (Rnd.PlayerCount != PlayerCount) R.RoundMark[i] = false;
            }
        }

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

        private void CheckHan(Result R)
        {
            for (int i = 0; i < R.Replay.Rounds.Count; i++)
            {
                Mahjong.Round Rnd = R.Replay.Rounds[i];

                for (int j = 0; j < 4; j++)
                {
                    if (HanMin != -1) if (Rnd.HanCount[j] < HanMin) R.HandMark[i][j] = false;
                    if (HanMax != -1) if (Rnd.HanCount[j] > HanMax) R.HandMark[i][j] = false;
                }
            }
        }

        private void CheckFu(Result R)
        {
            for (int i = 0; i < R.Replay.Rounds.Count; i++)
            {
                Mahjong.Round Rnd = R.Replay.Rounds[i];

                for (int j = 0; j < 4; j++)
                {
                    if (FuMin != -1) if (Rnd.FuCount[j] < FuMin) R.HandMark[i][j] = false;
                    if (FuMax != -1) if (Rnd.FuCount[j] > FuMax) R.HandMark[i][j] = false;
                }
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

        private void CheckLobby(Result R)
        {
            if (Rank == 0) return;

            if (R.Replay.Lobby != Lobby) R.ReplayMark = false;
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

        private void CheckDealer(Result R)
        {
            if (!Dealer) return;
            for (int i = 0; i < R.Replay.Rounds.Count; i++)
            {
                Mahjong.Round Rnd = R.Replay.Rounds[i];

                for (int j = 0; j < 4; j++)
                {
                    if (!Rnd.Dealer[j]) R.HandMark[i][j] = false;
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

        private void CheckDraw(Result R)
        {
            if (!Draw) return;

            for (int i = 0; i < R.Replay.Rounds.Count; i++)
            {
                Mahjong.Round Rnd = R.Replay.Rounds[i];
                if (Rnd.Result == Mahjong.RoundResult.Draw)
                {
                    if (DrawReason == Rnd.DrawReason)
                    {
                        switch (DrawReason)
                        {
                            case -1: continue; // normal;
                            case 0:            // 9 hai
                                int LastTsumoIndex = -1;
                                for (int j = 0; j < Rnd.Steps.Count; j++)
                                {
                                    if (Rnd.Steps[j].Type == Mahjong.StepType.STEP_DRAWTILE) LastTsumoIndex = j;
                                }

                                if (LastTsumoIndex >= 0)
                                {
                                    int Player = Rnd.Steps[LastTsumoIndex].Player;

                                    for (int j = 0; j < 4; j++)
                                    {
                                        if (j != Player) R.HandMark[i][j] = false;
                                    }
                                }
                                continue;
                            case 1: continue;  // reach 4
                            case 2: continue;  // ron 3
                            case 3: continue;  // kan 4
                            case 4: continue;  // kaze 4
                            case 5:            // nm
                                for (int j = 0; j < 4; j++)
                                {
                                    if (Rnd.Pay[j] < 0) R.HandMark[i][j] = false;
                                }
                                continue;
                        }
                    }
                }

                R.RoundMark[i] = false;
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

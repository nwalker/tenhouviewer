using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TenhouViewer.Mahjong;

namespace TenhouViewer.Search
{
    class BasicGameFinder
    {
        protected List<Result> GameList = new List<Result>();

        // Find queries
        // Fields use if they has not default values

        // Hash
        public string Hash = null;

        // Limit results, return last n results
        public int Last = -1;

        // Limit results, by count
        public int Limit = -1;

        public BasicGameFinder(Tenhou.TenhouHashList Hashes)
        {
            // Create blank ResultList from hash table
            for (int h = 0; h < Hashes.Hashes.Count; h++ )
            {
                string Hash = Hashes.Hashes[h];
                Mahjong.Replay R = new Mahjong.Replay();

                Console.Title = String.Format("Loading {0:d}/{1:d}", h + 1, Hashes.Hashes.Count);

                if (R.LoadXml(Hash))
                {
                    Result Res = new Result();
                    Res.Replay = R;
                    Res.ReplayMark = true;

                    for (int i = 0; i < 4; i++) Res.PlayerMark[i] = true;
                    // Exclude 4th player in 3man game
                    if (R.PlayerCount == 3) Res.PlayerMark[3] = false;

                    for (int i = 0; i < R.Rounds.Count; i++)
                    {
                        bool[] Marks = new bool[4];
                        for (int j = 0; j < Res.Replay.PlayerCount; j++) Marks[j] = true;

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
        public BasicGameFinder(List<Result> Results)
        {
            // Reset marks 
            foreach (Result R in Results)
            {
                R.ReplayMark = true;

                for (int i = 0; i < R.RoundMark.Count; i++)
                {
                    //R.RoundMark[i] = true;
                    for (int j = 0; j < R.Replay.PlayerCount; j++) R.HandMark[i][j] = true;
                }

                for (int i = 0; i < R.Replay.PlayerCount; i++) R.PlayerMark[i] = true;
                GameList.Add(R);
            }
        }

        public void ResetFlags()
        {
            // Reset marks 
            foreach (Result R in GameList)
            {
                R.ReplayMark = true;

                for (int i = 0; i < R.RoundMark.Count; i++)
                {
                    R.RoundMark[i] = true;
                    for (int j = 0; j < R.Replay.PlayerCount; j++) R.HandMark[i][j] = true;
                }

                for (int i = 0; i < R.Replay.PlayerCount; i++) R.PlayerMark[i] = true;
            }
        }

        protected virtual void CustomFilters(Result R)
        {

        }

        public List<Result> Find()
        {
            List<Result> ResultList = new List<Result>();
            int FoundGames = 0;

            for (int i = 0; i < GameList.Count; i++)
            {
                Result R = GameList[i];

                Console.Title = String.Format("Finding {0:d}/{1:d}, found {2:d}", i + 1, GameList.Count, ResultList.Count);

                if (Last != -1) if (i < GameList.Count - Last) continue;

                // filters
                CheckHash(R);
                CustomFilters(R);

                // Check mark
                EmbedMarksToHandMark(R);
                if (!IsQueryOk(R)) continue;

                if (Limit != -1)
                {
                    if (FoundGames >= Limit) break;
                    FoundGames++;
                }
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
                for (int j = 0; j < R.Replay.PlayerCount; j++)
                {
                    if (!R.PlayerMark[j]) R.HandMark[i][j] = false;
                    if (!R.RoundMark[i]) R.HandMark[i][j] = false;
                }

                // Exclude rounds which has no suitable hands
                R.RoundMark[i] = (R.RoundMark[i] && R.HandMark[i].Contains(true));
            }
        }

        private void CheckHash(Result R)
        {
            if (Hash == null) return;

            if (R.Replay.Hash.CompareTo(Hash) != 0) R.ReplayMark = false;
        }

        /// <summary>
        /// Проверить реплей
        /// </summary>
        /// <param name="R"></param>
        /// <param name="F"></param>
        protected void CheckReplay(Result R, Func<Replay,bool> F)
        {
            if (!F(R.Replay)) R.ReplayMark = false;
        }

        /// <summary>
        /// Проверить раздачи
        /// </summary>
        /// <param name="R"></param>
        /// <param name="F"></param>
        protected void CheckRounds(Result R, Func<Replay, Round, int, bool> F)
        {
            for (int i = 0; i < R.Replay.Rounds.Count; i++)
            {
                if (!F(R.Replay, R.Replay.Rounds[i], i)) R.RoundMark[i] = false;
            }
        }

        /// <summary>
        /// Проверить игроков
        /// </summary>
        /// <param name="R"></param>
        /// <param name="F"></param>
        protected void CheckPlayer(Result R, Func<Replay, Player, int, bool> F)
        {
            for (int i = 0; i < R.Replay.PlayerCount; i++)
            {
                if (!F(R.Replay, R.Replay.Players[i], i)) R.PlayerMark[i] = false;
            }
        }
    }
}

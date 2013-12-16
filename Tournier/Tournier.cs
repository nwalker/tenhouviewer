using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Tournier
{
    class Tournier
    {
        private List<Mahjong.Replay> Replays = new List<Mahjong.Replay>();
        private List<Result> Results = new List<Result>();

        public Tournier(Tenhou.TenhouHashList Hashes)
        {
            // Build result list
            for (int h = 0; h < Hashes.Hashes.Count; h++)
            {
                string Hash = Hashes.Hashes[h];
                Mahjong.Replay R = new Mahjong.Replay();

                Console.Title = String.Format("Loading {0:d}/{1:d}", h + 1, Hashes.Hashes.Count);

                if (R.LoadXml(Hash))
                {
                    Replays.Add(R);
                }
            }
        }

        public void Analyze()
        {
            // Analyze player's results
            for (int i = 0; i < Replays.Count; i++)
            {
                Mahjong.Replay R = Replays[i];

                for (int p = 0; p < R.PlayerCount; p++)
                {
                    Result Res = GetPlayerResult(R.Players[p].NickName);

                    Res.AddResult(R.Place[p], R.Balance[p]);
                }
            }
        }

        private Result GetPlayerResult(string Name)
        {
            for (int i = 0; i < Results.Count; i++)
            {
                Result R = Results[i];

                if (R.NickName.CompareTo(Name) == 0)
                {
                    // Player already exists in table
                    return R;
                }
            }

            // Not found. Create new:
            Result NewResult = new Result(Name);
            Results.Add(NewResult);

            return NewResult;
        }
    }
}

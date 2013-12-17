using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Tournier
{
    class Tournier
    {
        List<Search.Result> GamesList;
        private List<Result> Results = new List<Result>();

        public Tournier(List<Search.Result> Games)
        {
            GamesList = Games;
        }

        public List<Result> Analyze()
        {
            if (GamesList != null)
            {
                Results.Clear();
                // Analyze player's results
                for (int i = 0; i < GamesList.Count; i++)
                {
                    Mahjong.Replay R = GamesList[i].Replay;

                    for (int p = 0; p < R.PlayerCount; p++)
                    {
                        Result Res = GetPlayerResult(R.Players[p].NickName);

                        Res.AddResult(R.Place[p], R.Balance[p]);
                        Res.Replays.Add(R);
                    }
                }
            }

            return Results;
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

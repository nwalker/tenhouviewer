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

                        Res.AddResult(R.Place[p], R.Result[p], R.Balance[p]);
                        Res.Replays.Add(R);

                        Res.Rank = R.Players[p].Rank;
                        Res.Rating = R.Players[p].Rating;

                        for (int r = 0; r < R.Rounds.Count; r++)
                        {
                            Mahjong.Round Rnd = R.Rounds[r];

                            if (Rnd.Winner[p]) Res.AgariCount++;
                            if (Rnd.Loser[p]) Res.RonCount++;

                            if (Rnd.Pay[p] >= 0)
                            {
                                Res.TotalAcquisitions += Rnd.Pay[p];
                            }
                            else
                            {
                                Res.TotalLosses += Rnd.Pay[p];
                            }
                        }
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

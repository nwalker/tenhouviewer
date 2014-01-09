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

                        Res.AddResult(R.Place[p], R.Balance[p], R.Result[p]);
                        Res.Replays.Add(R);

                        Res.Rank = R.Players[p].Rank;
                        Res.Rating = R.Players[p].Rating;

                        for (int r = 0; r < R.Rounds.Count; r++)
                        {
                            Mahjong.Round Rnd = R.Rounds[r];

                            Res.RoundCount++;

                            if (Rnd.Winner[p])
                            {
                                // Calc yaku
                                for (int j = 0; j < Rnd.Yaku[p].Count; j++)
                                {
                                    Mahjong.Yaku Y = Rnd.Yaku[p][j];

                                    if (Y.Index > 51) // Dora
                                    {
                                        Res.Yaku[Y.Index] += Y.Cost;
                                    }
                                    else
                                    {
                                        Res.Yaku[Y.Index]++;
                                    }
                                }

                                Res.AgariCount++;
                            }
                            if (Rnd.Loser[p]) Res.RonCount++;

                            if (Rnd.Pay[p] >= 0)
                            {
                                if (Rnd.Result == Mahjong.RoundResult.Ron) Res.RonAcquisitions += Rnd.Pay[p];
                                else if (Rnd.Result == Mahjong.RoundResult.Draw) Res.DrawAcquisitions += Rnd.Pay[p];
                                else if (Rnd.Result == Mahjong.RoundResult.Tsumo) Res.TsumoAcquisitions += Rnd.Pay[p];

                                Res.TotalAcquisitions += Rnd.Pay[p];
                            }
                            else
                            {
                                if (Rnd.Loser[p]) Res.RonLosses += Rnd.Pay[p];
                                else if (Rnd.Result == Mahjong.RoundResult.Draw) Res.DrawLosses += Rnd.Pay[p];
                                else if (Rnd.Result == Mahjong.RoundResult.Tsumo) Res.TsumoLosses += Rnd.Pay[p];
                                Res.TotalLosses += Rnd.Pay[p];
                            }

                            if (HasFuriten(Rnd, p)) Res.Furiten++;
                            if (HasTempai(Rnd, p)) Res.Tempai++;

                            if (Rnd.Riichi[p] > -1)
                            {
                                Res.RiichiCount++;

                                if (Rnd.Winner[p])
                                {
                                    Res.RiichiWinCount++;

                                    for (int j = 0; j < Rnd.Yaku[p].Count; j++)
                                    {
                                        if (Rnd.Yaku[p][j].Index == 2)
                                        {
                                            Res.IppatsuCount++;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (Rnd.OpenedSets[p] > 0) Res.OpenedSetsCount++;
                        }
                    }
                }
            }

            return Results;
        }

        bool HasFuriten(Mahjong.Round Rnd, int Player)
        {
            for (int j = 0; j < Rnd.Steps.Count; j++)
            {
                if (Rnd.Steps[j].Player != Player) continue;

                if (Rnd.Steps[j].Furiten) return true;
            }

            return false;
        }

        bool HasTempai(Mahjong.Round Rnd, int Player)
        {
            for (int j = 0; j < Rnd.Steps.Count; j++)
            {
                if (Rnd.Steps[j].Player != Player) continue;

                if (Rnd.Steps[j].Shanten == 0) return true;
            }

            return false;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Tournier
{
    class Plotter
    {
        public List<string> Fields = new List<string>();
        private List<Result> ResultList = null;

        // Minimal game count to participate in table
        public int MinimalGamesCount = -1;

        public Plotter(List<Result> Results)
        {
            ResultList = Results;
        }

        public List<string> GamesGraph()
        {
            List<string> Output = new List<string>();
            int PlayerIndex = 1;
            int MaxGameCount = GetMaxGameCount();

            // Add fields name
            {
                string Temp = "";
                for (int i = 0; i < Fields.Count; i++)
                {
                    switch (Fields[i])
                    {
                        case "index": Temp += "Index\t"; break;
                        case "nickname": Temp += "NickName\t"; break;
                        case "rank": Temp += "Rank\t"; break;
                        case "rating": Temp += "Rating\t"; break;
                        case "games": Temp += "Games\t"; break;
                        case "placelist": Temp += "Places\t"; break;
                        case "place": Temp += "Avg. place\t"; break;
                        case "points": Temp += "Points\t"; break;
                        case "balance": Temp += "Balance\t"; break;
                        case "rounds": Temp += "Rounds\t"; break;

                        case "ron": Temp += "Dealt to ron\t"; break;
                        case "agari": Temp += "Hands\t"; break;

                        case "ronperc": Temp += "Dealt to ron perc\t"; break;
                        case "agariperc": Temp += "Completed perc\t"; break;

                        case "acq": Temp += "Acquisitions\t"; break;

                        case "acqron": Temp += "Acquisitions ron\t"; break;
                        case "acqdraw": Temp += "Acquisitions draw\t"; break;
                        case "acqtsumo": Temp += "Acquisitions tsumo\t"; break;

                        case "loss": Temp += "Losses\t"; break;

                        case "lossron": Temp += "Losses ron\t"; break;
                        case "lossdraw": Temp += "Losses draw\t"; break;
                        case "losstsumo": Temp += "Losses tsumo\t"; break;
                        case "lossriichi": Temp += "Losses riichi\t"; break;

                        case "1st": Temp += "1st place %\t"; break;
                        case "2nd": Temp += "2nd place %\t"; break;
                        case "3rd": Temp += "3rd place %\t"; break;
                        case "4th": Temp += "4th place %\t"; break;

                        case "furiten": Temp += "Furiten count\t"; break;
                        case "tempai": Temp += "Tempai count\t"; break;

                        case "riichi": Temp += "Riichi count\t"; break;
                        case "riichiwin": Temp += "Riichi win count\t"; break;
                        case "ippatsu": Temp += "Ippatsu count\t"; break;
                    }
                }

                Output.Add(Temp);
            }

            for (int i = 0; i < ResultList.Count; i++)
            {
                Result R = ResultList[i];
                string Temp = "";

                if (SkipThisResult(R)) continue;

                for (int j = 0; j < Fields.Count; j++)
                {
                    switch (Fields[j])
                    {
                        case "index": Temp += String.Format("{0:d}\t", PlayerIndex); break;
                        case "nickname": Temp += String.Format("{0:s}\t", R.NickName); break;
                        case "rank": Temp += String.Format("{0:s}\t", Tenhou.Rank.GetName(R.Rank)); break;
                        case "rating": Temp += String.Format("{0:d}R\t", R.Rating); break;
                        case "games": Temp += String.Format("{0:d}\t", R.Places.Count); break;
                        case "placelist": Temp += String.Format("{0:s}\t", R.GetPlaceList()); break;
                        case "place": Temp += String.Format("{0:f}\t", R.AveragePlace); break;
                        case "points": Temp += String.Format("{0:d}\t", R.TotalPoints); break;
                        case "balance": Temp += String.Format("{0:d}\t", R.TotalBalance); break;
                        case "ron": Temp += String.Format("{0:d}\t", R.RonCount); break;
                        case "agari": Temp += String.Format("{0:d}\t", R.AgariCount); break;
                        case "rounds": Temp += String.Format("{0:d}\t", R.RoundCount); break;

                        case "ronperc": Temp += String.Format("{0:f}\t", (R.RoundCount > 0) ? (100.0f * R.RonCount / R.RoundCount) : 0.0f); break;
                        case "agariperc": Temp += String.Format("{0:f}\t", (R.RoundCount > 0) ? (100.0f * R.AgariCount / R.RoundCount) : 0.0f); break;

                        case "acq": Temp += String.Format("+{0:d}\t", R.TotalAcquisitions); break;

                        case "acqron": Temp += String.Format("{0:d}\t", R.RonAcquisitions); break;
                        case "acqdraw": Temp += String.Format("{0:d}\t", R.DrawAcquisitions); break;
                        case "acqtsumo": Temp += String.Format("{0:d}\t", R.TsumoAcquisitions); break;

                        case "loss": Temp += String.Format("{0:d}\t", R.TotalLosses); break;

                        case "lossron": Temp += String.Format("{0:d}\t", R.RonLosses); break;
                        case "lossdraw": Temp += String.Format("{0:d}\t", R.DrawLosses); break;
                        case "losstsumo": Temp += String.Format("{0:d}\t", R.TsumoLosses); break;

                        case "1st": Temp += String.Format("{0:f}%\t", GetPlacePercent(R, 1)); break;
                        case "2nd": Temp += String.Format("{0:f}%\t", GetPlacePercent(R, 2)); break;
                        case "3rd": Temp += String.Format("{0:f}%\t", GetPlacePercent(R, 3)); break;
                        case "4th": Temp += String.Format("{0:f}%\t", GetPlacePercent(R, 4)); break;

                        case "furiten": Temp += String.Format("{0:d}\t", R.Furiten); break;
                        case "tempai": Temp += String.Format("{0:d}\t", R.Tempai); break;

                        case "riichi": Temp += String.Format("{0:d}\t", R.RiichiCount); break;
                        case "ippatsu": Temp += String.Format("{0:d}\t", R.IppatsuCount); break;
                        case "riichiwin": Temp += String.Format("{0:d}\t", R.RiichiWinCount); break;
                        case "lossriichi": Temp += String.Format("{0:d}\t", -1000 * (R.RiichiCount - R.RiichiWinCount)); break;
                    }
                }

                PlayerIndex++;
                Output.Add(Temp);
            }

            return Output;
        }

        public void Sort(string Parameter)
        {
            switch (Parameter)
            {
                case "place":
                    ResultList = ResultList.OrderBy(o => o.AveragePlace).ToList();
                    Console.WriteLine("Sort: by place;");
                    break;
                case "points": ResultList = ResultList.OrderBy(o => o.TotalPoints).ToList();
                    Console.WriteLine("Sort: by points;");
                    break;
                case "balance": ResultList = ResultList.OrderBy(o => o.TotalBalance).ToList();
                    Console.WriteLine("Sort: by balance;");
                    break;
                case "loss": ResultList = ResultList.OrderBy(o => o.TotalLosses).ToList();
                    Console.WriteLine("Sort: by losses;");
                    break;
                case "acq":
                    ResultList = ResultList.OrderBy(o => o.TotalAcquisitions).ToList();
                    Console.WriteLine("Sort: by acquisions;");
                    break;
                case "rank":
                    ResultList = ResultList.OrderBy(o => o.Rank).ToList();
                    Console.WriteLine("Sort: by rank;");
                    break;
                case "rating":
                    ResultList = ResultList.OrderBy(o => o.Rating).ToList();
                    Console.WriteLine("Sort: by rating;");
                    break;
            }
        }

        public void SortDescending(string Parameter)
        {
            switch (Parameter)
            {
                case "place":
                    ResultList = ResultList.OrderByDescending(o => o.AveragePlace).ToList();
                    Console.WriteLine("Sort descending: by place;");
                    break;
                case "points":
                    ResultList = ResultList.OrderByDescending(o => o.TotalPoints).ToList();
                    Console.WriteLine("Sort descending: by points;");
                    break;
                case "balance":
                    ResultList = ResultList.OrderByDescending(o => o.TotalBalance).ToList();
                    Console.WriteLine("Sort descending: by balance;");
                    break;
                case "loss":
                    ResultList = ResultList.OrderByDescending(o => o.TotalLosses).ToList();
                    Console.WriteLine("Sort descending: by losses;");
                    break;
                case "acq":
                    ResultList = ResultList.OrderByDescending(o => o.TotalAcquisitions).ToList();
                    Console.WriteLine("Sort descending: by acquisions;");
                    break;
                case "rank":
                    ResultList = ResultList.OrderByDescending(o => o.Rank).ToList();
                    Console.WriteLine("Sort descending: by rank;");
                    break;
                case "rating":
                    ResultList = ResultList.OrderByDescending(o => o.Rating).ToList();
                    Console.WriteLine("Sort descending: by rating;");
                    break;
            }
        }

        private bool SkipThisResult(Result R)
        {
            if (MinimalGamesCount != -1)
            {
                if (R.Places.Count < MinimalGamesCount) return true;
            }

            return false;
        }

        private int GetMaxGameCount()
        {
            int MaxValue = 0;
            for (int i = 0; i < ResultList.Count; i++)
            {
                Result R = ResultList[i];

                if (R.Places.Count > MaxValue) MaxValue = R.Places.Count;
            }

            return MaxValue;
        }

        private float GetPlacePercent(Result R, int Place)
        {
            int Count = 0;

            if (R.Places.Count == 0) return 0.0f;

            for (int i = 0; i < R.Places.Count; i++)
            {
                if (R.Places[i] == Place) Count++;
            }

            return 100.0f * Count / R.Places.Count;
        }
    }
}

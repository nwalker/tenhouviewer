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
                        case "placelist": Temp += String.Format("{0:s}\t", R.GetPlaceList()); break;
                        case "place": Temp += String.Format("{0:f}\t", R.AveragePlace); break;
                        case "points": Temp += String.Format("{0:d}\t", R.TotalPoints); break;
                        case "balance": Temp += String.Format("{0:d}\t", R.TotalBalance); break;
                        case "ron": Temp += String.Format("{0:d}\t", R.RonCount); break;
                        case "agari": Temp += String.Format("{0:d}\t", R.AgariCount); break;
                        case "acq": Temp += String.Format("+{0:d}\t", R.TotalAcquisitions); break;
                        case "loss": Temp += String.Format("{0:d}\t", R.TotalLosses); break;
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
                case "acq": ResultList = ResultList.OrderBy(o => o.TotalAcquisitions).ToList();
                    Console.WriteLine("Sort: by acquisions;");
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
    }
}

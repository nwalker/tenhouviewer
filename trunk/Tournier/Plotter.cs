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

        public Plotter(List<Result> Results)
        {
            ResultList = Results;
        }

        public List<string> GamesGraph()
        {
            List<string> Output = new List<string>();
            int PlayerIndex = 0;
            int MaxGameCount = GetMaxGameCount();

            for (int i = 0; i < ResultList.Count; i++)
            {
                Result R = ResultList[i];
                string Temp = "";

                for (int j = 0; j < Fields.Count; j++)
                {
                    switch (Fields[j])
                    {
                        case "index": Temp += String.Format("{0:d}\t", PlayerIndex); break;
                        case "nickname": Temp += String.Format("{0:s}\t", R.NickName); break;
                        case "placelist": break;
                        case "pointlist": break;
                        case "place": Temp += String.Format("{0:f}\t", R.AveragePlace); break;
                        case "points": Temp += String.Format("{0:d}\t", R.TotalPoints); break;
                    }
                }

                PlayerIndex++;
                Output.Add(Temp);
            }

            return Output;
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

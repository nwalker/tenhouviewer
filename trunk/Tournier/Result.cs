using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Tournier
{
    class Result
    {
        // Replay (for getting some additional information)
        public List<Mahjong.Replay> Replays = new List<TenhouViewer.Mahjong.Replay>();

        // Player's places
        public List<int> Places = new List<int>();

        // Player's total points
        public int TotalPoints = 0;

        // Player's total balance
        public int TotalBalance = 0;

        // Player's average place
        public float AveragePlace = 0.0f;

        // Player's to ron dealing count
        public int RonCount = 0;

        // Player's agari count
        public int AgariCount = 0;

        // Player's losses
        public int TotalLosses = 0;

        // Player's acquisition
        public int TotalAcquisitions = 0;

        // Player's nickname
        public string NickName = null;

        public Result(string Name)
        {
            NickName = Name;
        }

        public void AddResult(int Place, int Points, int Balance)
        {
            this.Places.Add(Place);

            float TempPlace = 0;
            for (int i = 0; i < this.Places.Count; i++) TempPlace += Places[i];

            TotalPoints += Points;
            TotalBalance += Balance * 100;
            AveragePlace = TempPlace / this.Places.Count;
        }

        public string GetPlaceList()
        {
            string Temp = "";
            for (int i = 0; i < Places.Count; i++) Temp += Places[i].ToString();

            return Temp;
        }
    }
}

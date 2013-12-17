using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Tournier
{
    class Result
    {
        // Replay
        public List<Mahjong.Replay> Replays = new List<TenhouViewer.Mahjong.Replay>();

        // Player's places
        public List<int> Places = new List<int>();

        // Player's points
        public List<int> Points = new List<int>();

        // Player's total points
        public int TotalPoints = 0;

        // Player's average place
        public float AveragePlace = 0.0f;

        // Player's nickname
        public string NickName = null;

        public Result(string Name)
        {
            NickName = Name;
        }

        public void AddResult(int Place, int Points)
        {
            this.Places.Add(Place);
            this.Points.Add(Points);

            float TempPlace = 0;
            int TempPoints = 0;
            for (int i = 0; i < this.Places.Count; i++)
            {
                TempPlace += Places[i];
                TempPoints += this.Points[i];
            }

            TotalPoints = TempPoints;
            AveragePlace = TempPlace / this.Places.Count;
        }
    }
}

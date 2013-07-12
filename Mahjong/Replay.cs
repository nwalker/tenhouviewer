using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Mahjong
{
    class Replay
    {
        public Player[] Players = new Player[4];
        private List<Round> Rounds = new List<Round>();

        public Replay()
        {

        }

        public void AddRound(Round Round)
        {
            Rounds.Add(Round);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Mahjong
{
    class Replay
    {
        private List<Player> Players = new List<Player>();
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

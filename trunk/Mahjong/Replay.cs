using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Mahjong
{
    class Replay
    {
        public string Hash = "";
        public Player[] Players = new Player[4];
        public List<Round> Rounds = new List<Round>();

        public Replay()
        {

        }

        public void Save()
        {
            // Save round info in files
            for (int i = 0; i < Rounds.Count; i++)
            {
                Rounds[i].Save("out/"+ Hash + "_" + i.ToString() + ".xml");
            }
        }
    }
}

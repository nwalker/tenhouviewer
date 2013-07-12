using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Mahjong
{
    class Round
    {
        public Wall Wall = new Wall();
        public List<Step> Steps = new List<Step>();
        public Hand[] Hands = new Hand[4]; // Start hands

        public Round()
        {
            
        }

        public void AddStep(Step Step)
        {
            Steps.Add(Step);
        }
    }
}

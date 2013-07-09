using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Mahjong
{
    class Round
    {
        private Wall Wall = new Wall();
        private List<Step> Steps = new List<Step>();
        private Hand[] HandList = new Hand[4]; // Start hands

        public Round()
        {
            
        }

        public void SetStartHands(Hand[] List)
        {
            for (int i = 0; i < 4; i++) HandList[i] = List[i];
        }
    }
}

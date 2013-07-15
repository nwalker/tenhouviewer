using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Mahjong
{
    enum NakiType
    {
        NOTYPE,
        CHI,
        PON,
        ANKAN,  // Concealed kan
        MINKAN, // Open kan
        CHAKAN, // Extended kan
        NUKI
    }

    class Naki
    {
        public int FromWho = -1;
        public NakiType Type = NakiType.NOTYPE;
        public List<int> Tiles = null;

        public Naki()
        {

        }
    }
}

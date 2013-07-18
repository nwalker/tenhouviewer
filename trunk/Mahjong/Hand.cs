using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Mahjong
{
    class Hand
    {
        public int[] Tiles = new int[14];

        public List<Naki> Naki = new List<Naki>();

        public Hand()
        {

        }

        public void SetArray(int[] Tiles)
        {
            for (int i = 0; i < 13; i++) this.Tiles[i] = Tiles[i];
            this.Tiles[13] = -1;

            Array.Sort(this.Tiles);
        }

        public int Shanten
        {
            get
            {
                ShantenCalculator SC = new ShantenCalculator(this);

                return SC.GetShanten();
            }
        }

        public void ReadXml(XmlLoad X)
        {
            if (!X.Read()) return;

            switch (X.ElementName)
            {
                case "tiles":
                    {
                        int Count = X.GetIntAttribute("count");
                        int Index = 0;

                        XmlLoad Subtree = X.GetSubtree();
                        for (int i = 0; i < Count; i++)
                        {
                            if (!Subtree.Read()) break;
                            switch (Subtree.ElementName)
                            {
                                case "tile":
                                    Tiles[Index] = Subtree.GetIntAttribute("value");
                                    Index++;
                                    break;
                            }
                        }
                    }
                    break;
                case "naki":
                    {
                        int Count = X.GetIntAttribute("count");

                        XmlLoad Subtree = X.GetSubtree();
                        for (int i = 0; i < Count; i++)
                        {
                            Naki N = new Naki();

                            N.ReadXml(X);

                            Naki.Add(N);
                        }
                    }
                    break;
            }
        }

        public void WriteXml(XmlSave X)
        {
            int TileCount = 0;
            for (int i = 0; i < Tiles.Length; i++) if (Tiles[i] >= 0) TileCount++;

            // Closed part
            X.StartTag("tiles");
            X.Attribute("count", TileCount);

            for(int i = 0; i < Tiles.Length; i++)
            {
                if (Tiles[i] < 0) continue;

                X.WriteTag("tile", "value", Tiles[i]); 
            }

            X.EndTag();

            // Opened part
            X.StartTag("naki");
            X.Attribute("count", Naki.Count);
            for (int i = 0; i < Naki.Count; i++)
            {
                Naki[i].WriteXml(X);
            }
            X.EndTag();
        }
    }
}

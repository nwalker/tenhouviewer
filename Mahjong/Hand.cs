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
            for (int i = 0; i < Tiles.Length; i++) Tiles[i] = -1;
        }

        public Hand(Hand H)
        {
            for (int i = 0; i < Tiles.Length; i++) this.Tiles[i] = H.Tiles[i];

            for (int i = 0; i < H.Naki.Count; i++) Naki.Add(H.Naki[i]);
        }

        public void SetArray(int[] Tiles)
        {
            for (int i = 0; i < 13; i++) this.Tiles[i] = Tiles[i];

            Array.Sort(this.Tiles);
        }

        public void Draw(int Tile)
        {
            for (int i = 0; i < Tiles.Length; i++)
            {
                if (Tiles[i] == -1)
                {
                    Tiles[i] = Tile;
                    break;
                }
            }

            Array.Sort(Tiles);
        }

        public void Discard(int Tile)
        {
            for (int i = 0; i < Tiles.Length; i++)
            {
                if (Tiles[i] == Tile) Tiles[i] = -1;
            }

            Array.Sort(Tiles);
        }

        public void OpenTiles(List<int> TileList)
        {
            foreach(int T in TileList)
            {
                for (int i = 0; i < Tiles.Length; i++)
                {
                    if (Tiles[i] == T) Tiles[i] = -1;
                }
            }
        }

        public int Shanten
        {
            get
            {
                ShantenCalculator SC = new ShantenCalculator(this);

                return SC.GetShanten();
            }
        }

        public List<int> WaitingList
        {
            get
            {
                ShantenCalculator SC = new ShantenCalculator(this);
                SC.GetShanten();

                return SC.WaitingList();
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
            if (Naki.Count > 0)
            {
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
}

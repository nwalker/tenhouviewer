using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

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

        public string GetText()
        {
            Tile[] TileList = new Tile[Tiles.Count];
            string[] TileName = new string[Tiles.Count];

            for (int i = 0; i < Tiles.Count; i++)
            {
                TileList[i] = new Tile(Convert.ToUInt16(Tiles[i]));
                TileName[i] = TileList[i].Value.ToString();
            }

            string TType = TileList[0].TileType;

            switch (Type)
            {
                case NakiType.NUKI:
                    return TileList[0].TileName;

                case NakiType.CHI:
                    {
                        string A = TileName[0];
                        string B = TileName[1];
                        string C = TileName[2];

                        return "[" + A + "]" + B + C + TType;
                    }
                case NakiType.PON:
                    {
                        string A = TileName[0];
                        string B = TileName[1];
                        string C = TileName[2];

                        switch (FromWho)
                        {
                            case 1: return A + B + "[" + C + "]" + TType;
                            case 2: return A + "[" + B + "]" + C + TType;
                            case 3: return "[" + A + "]" + B + C + TType;
                        }
                    }
                    break;
                case NakiType.ANKAN:
                    {
                        string A = TileName[0];
                        string B = TileName[1];
                        string C = TileName[2];
                        string D = TileName[2];
                        
                        return "[" + A + B + C + D + TType + "]";
                    }
                case NakiType.MINKAN:
                    {
                        string A = TileName[0];
                        string B = TileName[1];
                        string C = TileName[2];
                        string D = TileName[2];

                        switch (FromWho)
                        {
                            case 1: return A + B + C + "[" + D + "]" + TType;
                            case 2: return A + "[" + B + "]" + C + D + TType;
                            case 3: return "[" + A + "]" + B + C + D + TType;
                        }
                    }
                    break;
                case NakiType.CHAKAN:
                    {
                        string A = TileName[0];
                        string B = TileName[1];
                        string C = TileName[2];
                        string D = TileName[2];

                        switch (FromWho)
                        {
                            case 1: return A + B + "[" + C + "][" + D + "]" + TType;
                            case 2: return A + "[" + B + "][" + C + "]" + D + TType;
                            case 3: return "[" + A + "][" + B + "]" + C + D + TType;
                        }
                    }
                    break;
            }

            return "";
        }

        public void WriteXml(Xml X)
        {
            X.StartTag("type");
            X.Attribute("value", Convert.ToInt16(Type));
            X.Attribute("from", FromWho);
            X.Attribute("count", Tiles.Count);

            for (int i = 0; i < Tiles.Count; i++)
            {
                X.StartTag("tile");
                X.Attribute("value", Tiles[i]);

                X.EndTag();
            }

            X.EndTag();
        }
    }
}

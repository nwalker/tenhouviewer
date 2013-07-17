using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace TenhouViewer.Mahjong
{
    public enum StepType
    {
        STEP_DRAWTILE,     // Draw fram wall
        STEP_DISCARDTILE,  // Discard
        STEP_DRAWDEADTILE, // Draw from dead wall

        STEP_NAKI,         // Naki

        STEP_RIICHI,       // Riichi!
        STEP_RIICHI1000,   // Pay for riichi
        
        STEP_TSUMO,        // Tsumo
        STEP_RON,          // Ron
        STEP_DRAW,         // Draw

        STEP_NEWDORA,      // Open next dora indicator

        STEP_DISCONNECT,   // Player disconnected
        STEP_CONNECT       // Player connected
    }

    class Step
    {
        public int Player = -1;
        public int FromWho = -1;
        public int Tile = -1;
        public int Reason = -1;
        public StepType Type;
        public Naki NakiData = null;

        public Step(int Player)
        {
            this.Player = Player;
        }

        public void DrawTile(int Tile)
        {
            this.Tile = Tile;
            this.Type = StepType.STEP_DRAWTILE;

            Console.WriteLine(String.Format("[{0:d}] Draw {1:s}", Player, new Tile(Tile).TileName));
        }

        public void DrawDeadTile(int Tile)
        {
            this.Tile = Tile;
            this.Type = StepType.STEP_DRAWDEADTILE;

            Console.WriteLine(String.Format("[{0:d}] Draw tile from dead wall {1:s}", Player, new Tile(Tile).TileName));
        }

        public void DiscardTile(int Tile)
        {
            this.Tile = Tile;
            this.Type = StepType.STEP_DISCARDTILE;

            Console.WriteLine(String.Format("[{0:d}] Discard {1:s}", Player, new Tile(Tile).TileName));
        }

        public void Naki(Naki Naki)
        {
            this.NakiData = Naki;
            this.Type = StepType.STEP_NAKI;

            Console.WriteLine(String.Format("[{0:d}] Naki {1:s}", Player, Naki.GetText()));
        }

        public void DeclareRiichi()
        {
            this.Type = StepType.STEP_RIICHI;

            Console.WriteLine(String.Format("[{0:d}] Riichi: declare", Player));
        }

        public void PayRiichi()
        {
            this.Type = StepType.STEP_RIICHI1000;

            Console.WriteLine(String.Format("[{0:d}] Riichi: pay 1000", Player));
        }

        public void Ron(int From)
        {
            this.FromWho = From;
            this.Type = StepType.STEP_RON;

            Console.WriteLine(String.Format("[{0:d}] Ron on [{1:d}]", Player, From));
        }

        public void Tsumo()
        {
            this.Type = StepType.STEP_TSUMO;

            Console.WriteLine(String.Format("[{0:d}] Tsumo", Player));
        }

        public void Draw(int Reason)
        {
            this.Reason = Reason;
            this.Type = StepType.STEP_DRAW;

            Console.WriteLine("Draw");
        }

        public void NewDora(int Tile)
        {
            this.Tile = Tile;
            this.Type = StepType.STEP_NEWDORA;

            Console.WriteLine(String.Format("Dora {0:s}", new Tile(Tile).TileName));
        }

        public void Disconnect()
        {
            this.Type = StepType.STEP_DISCONNECT;

            Console.WriteLine(String.Format("[{0:d}] Disconnect", Player));
        }

        public void Connect()
        {
            this.Type = StepType.STEP_CONNECT;

            Console.WriteLine(String.Format("[{0:d}] Connect", Player));
        }

        public void ReadXml(XmlLoad X)
        {
            if(!X.Read()) return;

            switch (X.ElementName)
            {
                case "drawtile":
                    Player = X.GetIntAttribute("player");
                    Tile = X.GetIntAttribute("tile");

                    break;
                case "discardtile":
                    Player = X.GetIntAttribute("player");
                    Tile = X.GetIntAttribute("tile");
                    break;
                case "drawdeadtile":
                    Player = X.GetIntAttribute("player");
                    Tile = X.GetIntAttribute("tile");
                    break;
                case "naki":
                    Player = X.GetIntAttribute("player");
                    {
                        XmlLoad Subtree = X.GetSubtree();

                        NakiData = new Naki();
                        NakiData.ReadXml(X);
                    }
                    break;
                case "riichi1":
                    Player = X.GetIntAttribute("player");
                    break;
                case "riichi2":
                    Player = X.GetIntAttribute("player");
                    break;
                case "tsumo":
                    Player = X.GetIntAttribute("player");
                    break;
                case "ron":
                    Player = X.GetIntAttribute("player");
                    FromWho = X.GetIntAttribute("from");
                    break;
                case "draw":
                    Reason = X.GetIntAttribute("reason");
                    break;
                case "newdora":
                    Tile = X.GetIntAttribute("tile");
                    break;
                case "disconnect":
                    Player = X.GetIntAttribute("player");
                    break;
                case "connect":
                    Player = X.GetIntAttribute("player");
                    break;
            }
        }

        public void WriteXml(XmlSave X)
        {
            switch (Type)
            {
            case StepType.STEP_DRAWTILE:     // Draw fram wall
                X.StartTag("drawtile");
                X.Attribute("player", Player);
                X.Attribute("tile", Tile);

                X.EndTag();
                break;

            case StepType.STEP_DISCARDTILE:  // Discard
                X.StartTag("discardtile");
                X.Attribute("player", Player);
                X.Attribute("tile", Tile);

                X.EndTag();
                break;

            case StepType.STEP_DRAWDEADTILE: // Draw from dead wall
                X.StartTag("drawdeadtile");
                X.Attribute("player", Player);
                X.Attribute("tile", Tile);

                X.EndTag();
                break;

            case StepType.STEP_NAKI:         // Naki
                X.StartTag("naki");
                X.Attribute("player", Player);
                NakiData.WriteXml(X);
                X.EndTag();
                break;

            case StepType.STEP_RIICHI:       // Riichi!
                X.StartTag("riichi1");
                X.Attribute("player", Player);

                X.EndTag();
                break;

            case StepType.STEP_RIICHI1000:   // Pay for riichi
                X.StartTag("riichi2");
                X.Attribute("player", Player);

                X.EndTag();
                break;

            case StepType.STEP_TSUMO:        // Tsumo
                X.StartTag("tsumo");
                X.Attribute("player", Player);

                X.EndTag();
                break;

            case StepType.STEP_RON:          // Ron
                X.StartTag("ron");
                X.Attribute("player", Player);
                X.Attribute("from", FromWho);

                X.EndTag();
                break;
 
            case StepType.STEP_DRAW:         // Draw
                X.StartTag("draw");
                X.Attribute("reason", Reason);

                X.EndTag();
                break;

            case StepType.STEP_NEWDORA:      // Open next dora indicator
                X.StartTag("newdora");
                X.Attribute("tile", Tile);
                X.EndTag();
                break;

            case StepType.STEP_DISCONNECT:   // Player disconnected
                X.StartTag("disconnect");
                X.Attribute("player", Player);

                X.EndTag();
                break;

            case StepType.STEP_CONNECT:       // Player connected
                X.StartTag("connect");
                X.Attribute("player", Player);

                X.EndTag();
                break;
            }
        }
    }
}

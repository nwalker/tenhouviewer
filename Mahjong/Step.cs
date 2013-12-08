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

        public int[] Waiting = null;

        public Step(int Player)
        {
            this.Player = Player;
        }

        public void DrawTile(int Tile)
        {
            this.Tile = Tile;
            this.Type = StepType.STEP_DRAWTILE;
        }

        public void DrawDeadTile(int Tile)
        {
            this.Tile = Tile;
            this.Type = StepType.STEP_DRAWDEADTILE;
        }

        public void DiscardTile(int Tile)
        {
            this.Tile = Tile;
            this.Type = StepType.STEP_DISCARDTILE;
        }

        public void Naki(Naki Naki)
        {
            this.NakiData = Naki;
            this.Type = StepType.STEP_NAKI;
        }

        public void DeclareRiichi()
        {
            this.Type = StepType.STEP_RIICHI;
        }

        public void PayRiichi()
        {
            this.Type = StepType.STEP_RIICHI1000;
        }

        public void Ron(int From)
        {
            this.FromWho = From;
            this.Type = StepType.STEP_RON;
        }

        public void Tsumo()
        {
            this.Type = StepType.STEP_TSUMO;
        }

        public void Draw(int Reason)
        {
            this.Reason = Reason;
            this.Type = StepType.STEP_DRAW;
        }

        public void NewDora(int Tile)
        {
            this.Tile = Tile;
            this.Type = StepType.STEP_NEWDORA;
        }

        public void Disconnect()
        {
            this.Type = StepType.STEP_DISCONNECT;
        }

        public void Connect()
        {
            this.Type = StepType.STEP_CONNECT;
        }

        public void ReadXml(XmlLoad X)
        {
            if(!X.Read()) return;

            switch (X.ElementName)
            {
                case "drawtile":
                    Type = StepType.STEP_DRAWTILE;
                    Player = X.GetIntAttribute("player");
                    Tile = X.GetIntAttribute("tile");

                    break;
                case "discardtile":
                    Type = StepType.STEP_DISCARDTILE;
                    Player = X.GetIntAttribute("player");
                    Tile = X.GetIntAttribute("tile");
                    break;
                case "drawdeadtile":
                    Type = StepType.STEP_DRAWDEADTILE;
                    Player = X.GetIntAttribute("player");
                    Tile = X.GetIntAttribute("tile");
                    break;
                case "naki":
                    Type = StepType.STEP_NAKI;
                    Player = X.GetIntAttribute("player");
                    {
                        XmlLoad Subtree = X.GetSubtree();

                        NakiData = new Naki();
                        NakiData.ReadXml(X);
                    }
                    break;
                case "riichi1":
                    Type = StepType.STEP_RIICHI;
                    Player = X.GetIntAttribute("player");
                    break;
                case "riichi2":
                    Type = StepType.STEP_RIICHI1000;
                    Player = X.GetIntAttribute("player");
                    break;
                case "tsumo":
                    Type = StepType.STEP_TSUMO;
                    Player = X.GetIntAttribute("player");
                    break;
                case "ron":
                    Type = StepType.STEP_RON;
                    Player = X.GetIntAttribute("player");
                    FromWho = X.GetIntAttribute("from");
                    break;
                case "draw":
                    Type = StepType.STEP_DRAW;
                    Reason = X.GetIntAttribute("reason");
                    break;
                case "newdora":
                    Type = StepType.STEP_NEWDORA;
                    Tile = X.GetIntAttribute("tile");
                    break;
                case "disconnect":
                    Type = StepType.STEP_DISCONNECT;
                    Player = X.GetIntAttribute("player");
                    break;
                case "connect":
                    Type = StepType.STEP_CONNECT;
                    Player = X.GetIntAttribute("player");
                    break;
            }
        }

        private void WriteWaiting(XmlSave X)
        {
            if (Waiting == null) return;

            X.StartTag("waiting");
            for (int i = 0; i < Waiting.Length; i++)
            {
                X.StartTag("tiletype");
                X.Attribute("value", Waiting[i]);

                X.EndTag();
            }
            X.EndTag();
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
                WriteWaiting(X);

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
                WriteWaiting(X);
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

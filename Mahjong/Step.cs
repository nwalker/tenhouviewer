using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private int Player = -1;
        private int FromWho = -1;
        private int Tile = -1;
        private int Reason = -1;
        private StepType Type;
        private Naki NakiData = null;

        public Step(int Player)
        {
            this.Player = Player;
        }

        public void DrawTile(int Tile)
        {
            this.Tile = Tile;
            this.Type = StepType.STEP_DRAWTILE;

            Console.WriteLine("Draw tile");
        }

        public void DrawDeadTile(int Tile)
        {
            this.Tile = Tile;
            this.Type = StepType.STEP_DRAWDEADTILE;

            Console.WriteLine("Draw dead tile");
        }

        public void DiscardTile(int Tile)
        {
            this.Tile = Tile;
            this.Type = StepType.STEP_DISCARDTILE;

            Console.WriteLine("Discard");
        }

        public void Naki(Naki Naki)
        {
            this.NakiData = Naki;
            this.Type = StepType.STEP_NAKI;

            Console.WriteLine("Naki");
        }

        public void DeclareRiichi()
        {
            this.Type = StepType.STEP_RIICHI;

            Console.WriteLine("Riichi: declare");
        }

        public void PayRiichi()
        {
            this.Type = StepType.STEP_RIICHI1000;

            Console.WriteLine("Riichi: pay 1000");
        }

        public void Ron(int From)
        {
            this.FromWho = From;
            this.Type = StepType.STEP_RON;

            Console.WriteLine("Ron");
        }

        public void Tsumo()
        {
            this.Type = StepType.STEP_TSUMO;

            Console.WriteLine("Tsumo");
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

            Console.WriteLine("Dora");
        }

        public void Disconnect()
        {
            this.Type = StepType.STEP_DISCONNECT;

            Console.WriteLine("Disconnect");
        }

        public void Connect()
        {
            this.Type = StepType.STEP_CONNECT;

            Console.WriteLine("Connect");
        }
    }
}

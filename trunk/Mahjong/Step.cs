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

        STEP_CHITILE,     // Chi
        STEP_PONTILE,     // Pon
        STEP_MINKANTILE,  // Open kan
        STEP_ANKANTILE,   // Concealed kan
        STEP_CHAKANTILE,  // Extended kan

        STEP_RIICHI,      // Riichi!
        STEP_RIICHI1000,  // Pay for riichi
        
        STEP_TSUMO,       // Tsumo
        STEP_RON,         // Ron
        STEP_DRAW,        // Draw

        STEP_NEWDORA,     // Open next dora indicator

        STEP_DISCONNECT,  // Player disconnected
        STEP_CONNECT      // Player connected
    }

    class Step
    {
        private int Player = -1;
        private int FromWho = -1;
        private int Tile = -1;
        private int Reason = -1;
        private List<int> Set = null;
        private StepType Type;

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

        public void Chi(int Tile, List<int> Set, int From)
        {
            this.Tile = Tile;
            this.Set = Set;
            this.FromWho = From;
            this.Type = StepType.STEP_CHITILE;
        }

        public void Pon(int Tile, List<int> Set, int From)
        {
            this.Tile = Tile;
            this.Set = Set;
            this.FromWho = From;
            this.Type = StepType.STEP_PONTILE;
        }

        public void Minkan(int Tile, List<int> Set, int From)
        {
            this.Tile = Tile;
            this.Set = Set;
            this.FromWho = From;
            this.Type = StepType.STEP_MINKANTILE;
        }

        public void Ankan(int Tile, List<int> Set)
        {
            this.Tile = Tile;
            this.Set = Set;
            this.Type = StepType.STEP_ANKANTILE;
        }

        public void Chakan(int Tile, List<int> Set)
        {
            this.Tile = Tile;
            this.Set = Set;
            this.Type = StepType.STEP_CHAKANTILE;
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
            this.Type = StepType.STEP_DRAW;
        }

        public void Disconnect()
        {
            this.Type = StepType.STEP_DISCONNECT;
        }

        public void Connect()
        {
            this.Type = StepType.STEP_CONNECT;
        }
    }
}

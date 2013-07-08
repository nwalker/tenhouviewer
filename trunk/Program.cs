using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer
{
    class Program
    {
        static void Main(string[] args)
        {
            TestWall();
            TestHash();
        }

        static private void TestWall()
        {
            string Seed = "mt19937ar-sha512-n288-base64,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";

            Tenhou.WallGenerator WallG = new Tenhou.WallGenerator(Seed);

            WallG.Generate(0);
            int[] Wall = WallG.GetWall();
            int[] Dice = WallG.GetDice();
        }

        static private void TestHash()
        {
            Tenhou.Hash Hash = new Tenhou.Hash("2012090306gm-0089-0000-x666f4d41e26b");

            // Decoded: 2012090306gm-0089-0000-dc81a77a
        }
    }
}

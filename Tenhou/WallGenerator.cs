﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TenhouViewer.Tenhou
{
    class WallGenerator
    {
        // Random generator
        private MT19937AR R = new MT19937AR();

        private uint[] SeedValues = new uint[624];
        private int[] Wall = new int[136];
        private int[] Dice = new int[2];

        public WallGenerator(string Seed)
        {
            int DelimiterPos = Seed.IndexOf(",");

            string SeedText = Seed.Substring(DelimiterPos + 1);

            // mt19937ar-sha512-n288-base64,*data*
            byte[] SeedBytes;

            if (Seed.IndexOf("mt19937ar-sha512-n288-base64") == 0)
            {
                SeedBytes = Convert.FromBase64String(SeedText);

                // Convert to UInt32 array
                for (int i = 0; i < SeedValues.Length; i++) SeedValues[i] = (uint)(SeedBytes[i * 4 + 0] << 0) |
                                                                            (uint)(SeedBytes[i * 4 + 1] << 8) |
                                                                            (uint)(SeedBytes[i * 4 + 2] << 16) |
                                                                            (uint)(SeedBytes[i * 4 + 3] << 24);
            }
            else
            {
                // Old style seed value
                SeedValues = DecompositeHexList(SeedText);
            }
        }

        private uint[] DecompositeHexList(string Text)
        {
            string[] delimiter = new string[] { "," };
            string[] Temp;
            uint[] Result = null;

            if (Text == null) return null;

            Temp = Text.Split(delimiter, StringSplitOptions.None);
            Result = new uint[Temp.Length];

            for (int i = 0; i < Temp.Length; i++)
            {
                // Отрежем текст до точки
                int Index = Temp[i].IndexOf('.');
                if (Index >= 0) Temp[i] = Temp[i].Substring(0, Index);

                Result[i] = Convert.ToUInt32(Temp[i], 16);
            }

            return Result;
        }

        // Generate Wall by Index
        public void Generate(int Index)
        {
            // Init random generator by seed array
            R.InitByArray(SeedValues);

            uint[] rnd = new uint[64 / 4 * 9]; // 144

            // SHA512 hash
            for(int j = 0; j < (Index + 1); j++)
            {
                uint[] src = new uint[64 / 4 * 9 * 2];

                for (int i = 0; i < src.Length; i++) src[i] = R.GetNext();

                byte[] srcbyte = new byte[src.Length * 4];
                for (int i = 0; i < src.Length; i++)
                {
                    srcbyte[i * 4 + 0] = Convert.ToByte((src[i] >> 0) & 0xFF);
                    srcbyte[i * 4 + 1] = Convert.ToByte((src[i] >> 8) & 0xFF);
                    srcbyte[i * 4 + 2] = Convert.ToByte((src[i] >> 16) & 0xFF);
                    srcbyte[i * 4 + 3] = Convert.ToByte((src[i] >> 24) & 0xFF);
                }

                for (int i = 0; i < 9; ++i)
                {
                    SHA512Managed Context = new SHA512Managed();

                    byte[] datatohash = new byte[128];
                    for (int k = 0; k < 128; k++) datatohash[k] = srcbyte[k + i * 128];

                    byte[] hash = Context.ComputeHash(datatohash);

                    for (int k = 0; k < 16; k++)
                    {
                        rnd[i * 16 + k] = (uint)(hash[k * 4 + 0] << 0) |
                                          (uint)(hash[k * 4 + 1] << 8) |
                                          (uint)(hash[k * 4 + 2] << 16) |
                                          (uint)(hash[k * 4 + 3] << 24);
                    }
                }
            }

            // Generate wall
            for (int i = 0; i < Wall.Length; i++) Wall[i] = i;

            // Shuffle!
            for (int i = 0; i < Wall.Length - 1; i++)
            {
                int Src = i;
                int Dst = i + Convert.ToInt16(rnd[i] % (136 - i));

                // Swap 2 tiles
                int Swp = Wall[Src];
                Wall[Src] = Wall[Dst];
                Wall[Dst] = Swp;
            }

            // Dice!
            Dice[0] = 1 + Convert.ToInt16(rnd[135] % 6);
            Dice[1] = 1 + Convert.ToInt16(rnd[136] % 6);
        }

        public int[] GetWall()
        {
            int[] Temp = new int[136];
            for (int i = 0; i < Wall.Length; i++) Temp[i] = Wall[i];

            return Temp;
        }

        public int[] GetDice()
        {
            int[] Temp = new int[2];
            for (int i = 0; i < Dice.Length; i++) Temp[i] = Dice[i];

            return Temp;
        }
    }
}

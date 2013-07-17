using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Tenhou
{
    class TenhouHash
    {
        private string InternalHash;

        // Decode table
        int[] t = new int[] { 22136, 52719, 55146, 42104, 
                              59591, 46934, 9248,  28891,
                              49597, 52974, 62844, 4015,
                              18311, 50730, 43056, 17939,
                              64838, 38145, 27008, 39128,
                              35652, 63407, 65535, 23473,
                              35164, 55230, 27536, 4386,
                              64920, 29075, 42617, 17294,
                              18868, 2081};

        // Hash - типа
        // 2012090306gm-0089-0000-x666f4d41e26b - coded
        // 2012090306gm-0089-0000-dc81a77a - decoded
        public TenhouHash(string Game)
        {
            InternalHash = DecodeHash(Game);
        }

        private string DecodeHash(string Text)
        {
            int Pos = Text.LastIndexOf("-") + 1;
            string main = Text.Substring(Pos);

            if (main[0] == 'x')
            {
                // Coded hash
                int A = Convert.ToInt16(main.Substring(1, 4), 16);
                int B = Convert.ToInt16(main.Substring(5, 4), 16);
                int C = Convert.ToInt16(main.Substring(9, 4), 16);

                // Key from table
                int Index = 0;

                if (String.Compare(Text.Substring(0, 12), "2010041111gm") > 0)
                {
                    // New format
                    int X = Convert.ToInt32("3" + Text.Substring(4, 6));
                    int Y = Convert.ToInt16("" + Text[9]);
                    Index = X % (17 * 2 - Y - 1);
                }

                // Two parts of hash
                int First = (A ^ B ^ t[Index]) & 0xFFFF;
                int Second = (B ^ C ^ t[Index] ^ t[Index + 1]) & 0xFFFF;

                // Decode it!
                return Text.Substring(0, Pos) + String.Format("{0:x4}", First) + String.Format("{0:x4}", Second);
            }
            else
            {
                // Decoded hash.
                return Text;
            }
        }

        public string DecodedHash
        {
            get
            {
                return InternalHash;
            }
        }

        public string EncodedHash
        {
            get
            {
                int Pos = InternalHash.LastIndexOf("-") + 1;
                string main = DecodedHash.Substring(Pos);

                Random R = new Random();

                int A = R.Next(65536);
                int B = Convert.ToInt16(main.Substring(0, 4), 16);
                int C = Convert.ToInt16(main.Substring(4, 4), 16);
                int Index = 0;

                if (String.Compare(InternalHash.Substring(0, 12), "2010041111gm") > 0)
                {
                    // Новый формат
                    int X = Convert.ToInt32("3" + InternalHash.Substring(4, 6));
                    int Y = Convert.ToInt16("" + InternalHash[9]);
                    Index = X % (17 * 2 - Y - 1);
                }

                int First = A & 0xFFFF;
                int Second = (A ^ B ^ t[Index]) & 0xFFFF;
                int Third = (A ^ B ^ C ^ t[Index + 1]) & 0xFFFF;

                return InternalHash.Substring(0, Pos) + "x" + String.Format("{0:x4}", First) + String.Format("{0:x4}", Second) + String.Format("{0:x4}", Third);
            }
        }
    }
}

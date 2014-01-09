using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Tenhou
{
    static class LobbyType
    {
        public static string GetText(int Flags)
        {
            string Temp = "";
            int LobbyLevel = ((Flags & 0x0020) >> 4) | ((Flags & 0x0080) >> 7);

            bool Aka = (Flags & 0x0002) == 0;
            bool Kuitan = (Flags & 0x0004) == 0;
            bool Nan = (Flags & 0x0008) != 0;
            bool Sanma = (Flags & 0x0010) != 0;
            bool Saku = (Flags & 0x0040) != 0;
            bool Gray = (Flags & 0x0100) != 0;
            bool Chip = (Flags & 0x0200) != 0;
            bool Jans = (Flags & 0x0400) != 0;
            bool Tech = (Flags & 0x0800) != 0;

            // Sanma
            if (Sanma) Temp += "三";

            // Lobby level
            switch (LobbyLevel)
            {
                case 0: Temp += "般"; break; // 一般, ippan; general lobby
                case 1: Temp += "上"; break; // 上級, uekyuu; 1dan+ lobby
                case 2: Temp += "特"; break; // 特上, tokujou; 1800R 4dan+ lobby
            }

            if (Tech)
            {
                // Length
                Temp += (Nan) ? "南" : "東";
            }

            // Open tanyao
            Temp += (Kuitan) ? "喰" : "";

            // Aka-dora
            Temp += (Aka) ? "赤" : "";

            if (Jans)
            {
                // Gray (wtf?)
                Temp += (Gray) ? "速" : "";

                if (!Chip)
                {
                    Temp += "祝０";
                }
                else
                {
                    Temp += (Jans) ? "祝５" : "祝２";
                }
            }

            // Saku
            Temp += (Saku) ? "速" : "";

            // Chip
            Temp += (Chip) ? "祝" : "";

            return Temp;
        }
    }
}

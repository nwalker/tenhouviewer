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

            // Sanma
            if ((Flags & 0x0010) != 0) Temp += "三";

            // Lobby level
            switch (LobbyLevel)
            {
                case 0: Temp += "般"; break; // 一般, ippan; general lobby
                case 1: Temp += "上"; break; // 上級, uekyuu; 1dan+ lobby
                case 2: Temp += "特"; break; // 特上, tokujou; 1800R 4dan+ lobby
            }

            // Length
            Temp += ((Flags & 0x0008) != 0) ? "南" : "東";

            // Open tanyao
            Temp += ((Flags & 0x0004) == 0) ? "喰" : "";

            // Aka-dora
            Temp += ((Flags & 0x0002) == 0) ? "赤" : "";

            // Saku
            Temp += ((Flags & 0x0040) == 0) ? "速" : "";

            // Gray (wtf?)
            Temp += ((Flags & 0x0100) == 0) ? "速" : "";

            // Chip (wtf?)
            Temp += ((Flags & 0x0200) == 0) ? "祝" : "";

            return Temp;
        }
    }
}

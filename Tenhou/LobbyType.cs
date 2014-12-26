using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Tenhou
{
    static class LobbyType
    {
        public static string GetText(Mahjong.LobbyType Flags)
        {
            string Temp = "";
            int F = Convert.ToInt32(Flags);
            int LobbyLevel = ((F & 0x0020) >> 4) | ((F & 0x0080) >> 7);

            bool Aka = !Flags.HasFlag(Mahjong.LobbyType.NOAKA);
            bool Kuitan = !Flags.HasFlag(Mahjong.LobbyType.NOKUI);
            bool Nan = Flags.HasFlag(Mahjong.LobbyType.NAN);
            bool Sanma = Flags.HasFlag(Mahjong.LobbyType.SANMA);
            bool Saku = Flags.HasFlag(Mahjong.LobbyType.SAKU);
            bool Gray = Flags.HasFlag(Mahjong.LobbyType.GRAY);
            bool Chip = Flags.HasFlag(Mahjong.LobbyType.CHIP);
            bool Jans = Flags.HasFlag(Mahjong.LobbyType.JANS);
            bool Tech = Flags.HasFlag(Mahjong.LobbyType.TECH);

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

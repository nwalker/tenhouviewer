using System;
using System.Collections.Generic;
using System.Text;

namespace TenhouViewer.Mahjong
{
    static class YakuName
    {
        static string[] YakuList = new string[55];

        static YakuName()
        {
            YakuList[0] = "Menzen Tsumo";
            YakuList[1] = "Riichi";
            YakuList[2] = "Ippatsu";
            YakuList[3] = "Chankan";
            YakuList[4] = "Rinshan Kaihou";
            YakuList[5] = "Haitei Raoyue";
            YakuList[6] = "Houtei Raoyui";
            YakuList[7] = "Pinfu";
            YakuList[8] = "Tanyao";
            YakuList[9] = "Ippeiko";
            YakuList[10] = "Fanpai 東 (seat wind)";
            YakuList[11] = "Fanpai 南 (seat wind)";
            YakuList[12] = "Fanpai 西 (seat wind)";
            YakuList[13] = "Fanpai 北 (seat wind)";
            YakuList[14] = "Fanpai 東 (round wind)";
            YakuList[15] = "Fanpai 南 (round wind)";
            YakuList[16] = "Fanpai 西 (round wind)";
            YakuList[17] = "Fanpai 北 (round wind)";
            YakuList[18] = "Yakuhai 白";
            YakuList[19] = "Yakuhai 發";
            YakuList[20] = "Yakuhai 中";

            // 二飜
            YakuList[21] = "Double Riichi";
            YakuList[22] = "Chiitoitsu";
            YakuList[23] = "Chanta";
            YakuList[24] = "Itsuu";
            YakuList[25] = "Sanshoku Doujun";
            YakuList[26] = "Sanshoku Dou";
            YakuList[27] = "Sankantsu";
            YakuList[28] = "Toi-Toi";
            YakuList[29] = "Sanankou";
            YakuList[30] = "Shousangen";
            YakuList[31] = "Honrouto";

            //  三飜
            YakuList[32] = "Ryanpeikou";
            YakuList[33] = "Jun Chan";
            YakuList[34] = "Honitsu";

            //  六飜
            YakuList[35] = "Chinitsu";

            //  満貫
            YakuList[36] = "Renhou";

            //  役満
            YakuList[37] = "Tenhou";
            YakuList[38] = "Chiihou";
            YakuList[39] = "Daisangen";
            YakuList[40] = "Suuankou";
            YakuList[41] = "Suuankou Tanki";
            YakuList[42] = "Tsuiisou";
            YakuList[43] = "Ryuuiisou";
            YakuList[44] = "Chinrouto";
            YakuList[45] = "Chuuren Pooto";
            YakuList[46] = "Chuuren Pooto 9 wait";
            YakuList[47] = "Kokushi Musou";
            YakuList[48] = "Kokushi Musou 13 wait";
            YakuList[49] = "Daisuushi";
            YakuList[50] = "Shousuushi";
            YakuList[51] = "Suukantsu";
            YakuList[52] = "Dora";
            YakuList[53] = "Ura-dora";
            YakuList[54] = "Aka-dora";
        }

        static public string GetYakuName(int Index)
        {
            if (Index >= YakuList.Length) return "INCORRECT INDEX " + Convert.ToString(Index);
            if (YakuList[Index] == null) return "NO NAME " + Convert.ToString(Index);

            return YakuList[Index];
        }
    }
}

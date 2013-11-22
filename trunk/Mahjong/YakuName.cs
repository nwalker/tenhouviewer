using System;
using System.Collections.Generic;
using System.Text;

namespace TenhouViewer.Mahjong
{
    static class YakuName
    {
       // static string[] YakuList = new string[55];

        static string[] YakuList = {"Menzen Tsumo", "Riichi", "Ippatsu", "Chankan", "Rinshan Kaihou", "Haitei Raoyue", "Houtei Raoyui",
                                    "Pinfu", "Tanyao", "Ippeiko", "Fanpai 東 (seat wind)", "Fanpai 南 (seat wind)", "Fanpai 西 (seat wind)",
                                    "Fanpai 北 (seat wind)", "Fanpai 東 (round wind)", "Fanpai 南 (round wind)", "Fanpai 西 (round wind)",
                                    "Fanpai 北 (round wind)", "Yakuhai 白", "Yakuhai 發", "Yakuhai 中", "Double Riichi", "Chiitoitsu",
                                    "Chanta", "Itsuu", "Sanshoku Doujun", "Sanshoku Dou", "Sankantsu", "Toi-Toi", "Sanankou", "Shousangen",
                                    "Honrouto", "Ryanpeikou", "Jun Chan", "Honitsu", "Chinitsu", "Renhou", "Tenhou", "Chiihou", "Daisangen",
                                    "Suuankou", "Suuankou Tanki", "Tsuiisou", "Ryuuiisou", "Chinrouto", "Chuuren Pooto", "Chuuren Pooto 9 wait",
                                    "Kokushi Musou", "Kokushi Musou 13 wait", "Daisuushi", "Shousuushi", "Suukantsu", "Dora", "Ura-dora", "Aka-dora"
                                   };

        static YakuName()
        {

        }

        static public string GetYakuName(int Index)
        {
            if (Index >= YakuList.Length) return "INCORRECT INDEX " + Convert.ToString(Index);
            if (YakuList[Index] == null) return "NO NAME " + Convert.ToString(Index);

            return YakuList[Index];
        }
    }
}

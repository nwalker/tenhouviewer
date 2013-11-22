using System;
using System.Collections.Generic;
using System.Text;

namespace TenhouViewer.Mahjong
{
    class YakuLanguagePack
    {
        public string Language;
        public string[] List;

        public YakuLanguagePack(string L, string[] List)
        {
            this.Language = L;
            this.List = List;
        }
    }

    static class YakuName
    {
       // static string[] YakuList = new string[55];

        static string[] YakuListEn = {"Menzen Tsumo", "Riichi", "Ippatsu", "Chankan", "Rinshan Kaihou", "Haitei Raoyue", "Houtei Raoyui",
                                      "Pinfu", "Tanyao", "Ippeiko", "Fanpai 東 (seat wind)", "Fanpai 南 (seat wind)", "Fanpai 西 (seat wind)",
                                      "Fanpai 北 (seat wind)", "Fanpai 東 (round wind)", "Fanpai 南 (round wind)", "Fanpai 西 (round wind)",
                                      "Fanpai 北 (round wind)", "Yakuhai 白", "Yakuhai 發", "Yakuhai 中", "Double Riichi", "Chiitoitsu",
                                      "Chanta", "Itsuu", "Sanshoku Doujun", "Sanshoku Dou", "Sankantsu", "Toi-Toi", "Sanankou", "Shousangen",
                                      "Honrouto", "Ryanpeikou", "Jun Chan", "Honitsu", "Chinitsu", "Renhou", "Tenhou", "Chiihou", "Daisangen",
                                      "Suuankou", "Suuankou Tanki", "Tsuiisou", "Ryuuiisou", "Chinrouto", "Chuuren Pooto", "Chuuren Pooto 9 wait",
                                      "Kokushi Musou", "Kokushi Musou 13 wait", "Daisuushi", "Shousuushi", "Suukantsu", "Dora", "Ura-dora", "Aka-dora"
                                   };

        static string[] YakuListJp = {"門前清自摸和", "立直", "一発", "槍槓", "嶺上開花", "海底摸月", "河底撈魚",
                                      "平和", "断幺九", "一盃口", "自風 東", "自風 南", "自風 西",
                                      "自風 北", "場風 東", "場風 南", "場風 西)",
                                      "場風 北", "役牌 白", "役牌 發", "役牌 中", "両立直", "七対子",
                                      "混全帯幺九", "一気通貫", "三色同順", "三色同刻", "三槓子", "対々和", "三暗刻", "小三元",
                                      "混老頭", "二盃口", "純全帯幺九", "混一色", "清一色", "人和", "天和", "地和", "大三元",
                                      "四暗刻", "四暗刻単騎", "字一色", "緑一色", "清老頭", "九蓮宝燈", "純正九蓮宝燈",
                                      "国士無双", "国士無双１３面", "大四喜", "小四喜", "四槓子", "ドラ", "裏ドラ", "赤ドラ"
                                     };

        static string[] YakuListRu = {"Цумо", "риичи", "Иппацу", "Чанкан", "Риншан", "Хайтей", "Хотей",
                                      "Пинфу", "Таняо", "Иппейко", "Фанпай 東 (место)", "Фанпай 南 (место)", "Фанпай 西 (место)",
                                      "Фанпай 北 (место)", "Фанпай 東 (раунд)", "Фанпай 南 (раунд)", "Фанпай 西 (раунд)",
                                      "Фанпай 北 (раунд)", "Якухай 白", "Якухай 發", "Якухай 中", "Дабури", "Читойцу",
                                      "Чанта", "Иццу", "Саншоку доджин", "Саншоку доко", "Санканцу", "Тойтой", "Сананко", "Сёсанген",
                                      "Хонрото", "Рянпейко", "Джунчан", "Хоницу", "Чиницу", "Ренхо", "Тенхо", "Чихо", "Дайсанген",
                                      "Сууанко", "Сууанко танки", "Цуисо", "Рюисо", "Чинрото", "Чууренпото", "Чууренпото 9 сторон",
                                      "Кокуши", "Кокуши 13 сторон", "Дайсуши", "Сёсуши", "Сууканцу", "Дора", "Урадора", "Акадора"
                                   };

        static YakuLanguagePack[] YakuPack = new YakuLanguagePack[] {
            new YakuLanguagePack("en", YakuListEn),
            new YakuLanguagePack("jp", YakuListJp),
            new YakuLanguagePack("ru", YakuListRu),
        };

        static YakuName()
        {

        }

        static public string GetYakuName(string Lang, int Index)
        {
            int LangIndex = 0;
            for (int i = 0; i < YakuPack.Length; i++)
            {
                if (Lang == YakuPack[i].Language)
                {
                    LangIndex = i;
                    break;
                }
            }

            if (Index >= YakuPack[LangIndex].List.Length) return "INCORRECT INDEX " + Convert.ToString(Index);
            if (YakuPack[LangIndex].List[Index] == null) return "NO NAME " + Convert.ToString(Index);

            return YakuPack[LangIndex].List[Index];
        }
    }
}

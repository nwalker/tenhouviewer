using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Tenhou
{
    class YakuNameItem
    {
        public string Name;
        public int[] Indexes;

        public YakuNameItem(string N, int[] I)
        {
            Name = N;
            Indexes = I;
        }
    }

    static class YakuNameParser
    {
        private static YakuNameItem[] YakuNameList = new YakuNameItem[] {
            new YakuNameItem("tsumo", new int[] {0}),
            new YakuNameItem("riichi", new int[] {1}),
            new YakuNameItem("ippatsu", new int[] {2}),
            new YakuNameItem("chankan", new int[] {3}),
            new YakuNameItem("rinshan", new int[] {4}),
            new YakuNameItem("haitei", new int[] {5}),
            new YakuNameItem("houtei", new int[] {6}),
            new YakuNameItem("pinfu", new int[] {7}),
            new YakuNameItem("tanyao", new int[] {8}),
            new YakuNameItem("ippeiko", new int[] {9}),
            new YakuNameItem("fanpai", new int[] {10, 11, 12, 13, 14, 15, 16, 17 }),
            new YakuNameItem("yakuhai", new int[] {18, 19, 20}),
            new YakuNameItem("daburi", new int[] {21}),
            new YakuNameItem("chiitoi", new int[] {22}),
            new YakuNameItem("chanta", new int[] {23}),
            new YakuNameItem("itsuu", new int[] {24}),
            new YakuNameItem("sanshokudoujin", new int[] {25}),
            new YakuNameItem("sanshokudou", new int[] {26}),
            new YakuNameItem("sankantsu", new int[] {27}),
            new YakuNameItem("toitoi", new int[] {28}),
            new YakuNameItem("sanankou", new int[] {29}),
            new YakuNameItem("shousangen", new int[] {30}),
            new YakuNameItem("honrouto", new int[] {31}),
            new YakuNameItem("ryanpeikou", new int[] {32}),
            new YakuNameItem("junchan", new int[] {33}),
            new YakuNameItem("honitsu", new int[] {34}),
            new YakuNameItem("chinitsu", new int[] {35}),
            new YakuNameItem("renhou", new int[] {36}),
            new YakuNameItem("tenhou", new int[] {37}),
            new YakuNameItem("chihou", new int[] {38}),
            new YakuNameItem("daisangen", new int[] {39}),
            new YakuNameItem("suuankou", new int[] {40, 41}),
            new YakuNameItem("tsuiisou", new int[] {42}),
            new YakuNameItem("ryuuiisou", new int[] {43}),
            new YakuNameItem("chinrouto", new int[] {44}),
            new YakuNameItem("chuurenpooto", new int[] {45, 46}),
            new YakuNameItem("kokushi", new int[] {47, 48}),
            new YakuNameItem("daisuushi", new int[] {49}),
            new YakuNameItem("shousuushi", new int[] {50}),
            new YakuNameItem("suukantsu", new int[] {51}),
            new YakuNameItem("dora", new int[] {52}),
            new YakuNameItem("uradora", new int[] {53}),
            new YakuNameItem("akadora", new int[] {54}),
        };

        private static bool IsInList(List<int> List, int Index)
        {
            return List.Contains(Index);
        }

        public static int[] Parse(string[] List)
        {
            List<int> YakuList = new List<int>();

            foreach (string Item in List)
            {
                // Is index?
                try
                {
                    int Index = Convert.ToInt32(Item);

                    if ((Index < 54) && (Index >= 0)) if (!IsInList(YakuList, Index)) YakuList.Add(Index);
                }
                catch(Exception)
                {
                    foreach (YakuNameItem YakuName in YakuNameList)
                    {
                        if (YakuName.Name.CompareTo(Item) == 0)
                        {
                            foreach (int Index in YakuName.Indexes) if (!IsInList(YakuList, Index)) YakuList.Add(Index);
                        }
                    }
                }
            }

            return YakuList.ToArray();
        }
    }
}

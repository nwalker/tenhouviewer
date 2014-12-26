using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TenhouViewer.Mahjong
{
    [Flags]
    enum LobbyType
    {
        MULTI = 0x0001, // versus player
        NOAKA = 0x0002, // no red
        NOKUI = 0x0004, // no open tanyao
        NAN   = 0x0008, // east and south round (lit. east south)
        SANMA = 0x0010, // 3-man
        TOKU  = 0x0020, // expert class
        SAKU  = 0x0040, // speedy (half wait time)
        HIGH  = 0x0080, // advanced class
        GRAY  = 0x0100, // ??
        CHIP  = 0x0200, // ??
        JANS  = 0x0400, // ??
        TECH  = 0x0800, // ??
    }
}

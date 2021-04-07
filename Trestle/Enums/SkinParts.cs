using System;

namespace Trestle.Enums
{
    [Flags]
    public enum SkinParts
    {
        Cape = 0x01,
        Jacket = 0x02,
        LeftSleeve = 0x04,
        RightSleeve = 0x08,
        LeftPants = 0x10,
        RightPants = 0x20,
        Hat = 0x40
    }
}
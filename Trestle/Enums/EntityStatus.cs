using System;

namespace Trestle.Enums
{
    [Flags]
    public enum EntityStatus
    {
        OnFire = 0x01,
        Crouched = 0x02,
        Unused1 = 0x04,
        Sprinting = 0x08,
        Unused2 = 0x10,
        Invisible = 0x20,
        Glowing = 0x40,
        ElytraFlying = 0x80
    }
}
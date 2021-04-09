using System.ComponentModel;

namespace Trestle.Enums
{
    public enum DamageCause
    {
        [Description("{0} fell out of the world")] Unknown,
        [Description("{0} hugged a cactus")] Cactus,
        [Description("{0} was shot by {1}")] Projectile,
        [Description("{0} dug straight up")] Suffocation,
        [Description("{0} didn't stick the landing")] Falling,
        [Description("{0} is §oliterally §ron fire")] Fire,
        [Description("{0} thought the lava was orange juice")] Lava,
        [Description("{0} thought they could breathe underwater")] Drowning,
        [Description("{0} went out with a §lBANG!")] Explosion,
        [Description("{0} wasn't prepared for witchcraft")] Magic
    }
}
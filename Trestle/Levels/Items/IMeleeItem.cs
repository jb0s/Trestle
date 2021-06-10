namespace Trestle.Levels.Items
{
    public interface IMeleeItem : IItem
    {
        /// <summary>
        /// The amount of damage it deals when fully charged.
        /// </summary>
        public float AttackDamage => 1;
        
        /// <summary>
        /// The amount of ticks it takes to pull out this weapon and have it fully charged.
        /// </summary>
        public int PulloutTime => 1;
    }
}
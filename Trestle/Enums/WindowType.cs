using Trestle.Attributes;

namespace Trestle.Enums
{
    public enum WindowType
    {
        [Description("minecraft:container")] Container = 1,
        [Description("minecraft:chest")] Chest,
        [Description("minecraft:crafting_table")] CraftingTable,
        [Description("minecraft:furnace")] Furnace,
        [Description("minecraft:dispenser")] Dispenser,
        [Description("minecraft:enchanting_table")] EnchantingTable,
        [Description("minecraft:brewing_stand")] BrewingStand,
        [Description("minecraft:villager")] Villager,
        [Description("minecraft:beacon")] Beacon,
        [Description("minecraft:anvil")] Anvil,
        [Description("minecraft:hopper")] Hopper,
        [Description("minecraft:dropper")] Dropper,
        [Description("minecraft:shulker_box")] ShulkerBox,
        [Description("EntityHorse")] EntityHorse
    }
}
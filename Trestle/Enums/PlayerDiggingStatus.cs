namespace Trestle.Enums
{
    public enum PlayerDiggingStatus : byte
    {
        StartedDigging = 0x00,
        CancelledDigging = 0x01,
        FinishedDigging = 0x02,
        DropItemStack = 0x03,
        DropItem = 0x04,
        ShootArrow = 0x05,
        FinishEating = 0x05,
        SwapItemInHand = 0x06
    }
}
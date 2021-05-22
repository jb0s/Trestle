namespace Trestle.Networking.Enums.Client
{
    public enum PlayPacket : byte
    {
        JoinGame = 0x24,
        PlayerPositionAndLook = 0x34,
        SpawnPosition = 0x42,
    }
}
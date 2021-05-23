namespace Trestle.Networking.Enums.Client
{
    public enum PlayPacket : byte
    {
        JoinGame = 0x24,
        PlayerPositionAndLook = 0x34,
        SpawnPosition = 0x42,
        KeepAlive = 0x1F,
        ChunkData = 0x20
    }
}
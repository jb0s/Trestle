namespace Trestle.Enums.Packets.Client
{
    public enum PlayPacket : byte
    {
        KeepAlive = 0x1F,
        ChatMessage = 0x0F,
        EntityEquipment = 0x3F,
        SpawnPosition = 0x46,
        PlayerPositionAndLook = 0x2F,
        Animation = 0x06,
        CollectItem = 0x4B,
        SpawnObject = 0x00,
        DestroyEntities = 0x32,
        EntityMetadata = 0x3C,
        ChunkData = 0x20,
        JoinGame = 0x23,
        NamedSoundEffect = 0x19,
        ChangeGameState = 0x1E,
        SetSlot = 0x16,
        PlayerListItem = 0x2E,
        Disconnect = 0x1A,
        BlockChange = 0x0B,
        TabComplete = 0x0E,
        SpawnPlayer = 0x05,
        EntityRelativeMove = 0x28,
        EntityLook = 0x2B,
        EntityLookAndRelativeMove = 0x29
    }
}
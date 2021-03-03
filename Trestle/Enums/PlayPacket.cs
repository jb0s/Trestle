using Trestle.Attributes;
using Trestle.Networking;

namespace Trestle.Enums
{
    public enum PlayPacket : byte
    {
        // Serverbound
        Server_PlayerPositionAndLook = 0x13,

        // Clientbound
        Client_PlayerPositionAndLook = 0x34,
        EntityStatus = 0x1A,
        SetSlot = 0x15,
        ChunkData = 0x20,
        JoinGame = 0x24,
        SpawnPosition = 0x42,
    }
}
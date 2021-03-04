using Trestle.Attributes;
using Trestle.Networking;

namespace Trestle.Enums
{
    public enum PlayPacket : byte
    {
        // Serverbound
        Server_PlayerPositionAndLook = 0x13,
        Server_ChatMessage = 0x03,
        Server_KeepAlive = 0x10,
        
        // Clientbound
        Client_PlayerPositionAndLook = 0x34,
        Client_ChatMessage = 0x0E,
        Client_KeepAlive = 0x1F,
        
        EntityStatus = 0x1A,
        SetSlot = 0x15,
        ChunkData = 0x20,
        JoinGame = 0x24,
        SpawnPosition = 0x42,
    }
}
using Trestle.Attributes;
using Trestle.Networking;

namespace Trestle.Enums
{
    public enum PlayPacket : byte
    {
        // Serverbound
        Server_PlayerPositionAndLook = 0x06,
        Server_ChatMessage = 0x01,
        Server_KeepAlive = 0x00,
        Server_Player = 0x03,
        Server_PlayerPosition = 0x04,
        
        // Clientbound
        Client_PlayerPositionAndLook = 0x08,
        Client_ChatMessage = 0x02,
        Client_KeepAlive = 0x00,
        
        EntityStatus = 0x1A,
        SetSlot = 0x2F,
        ChunkData = 0x21,
        JoinGame = 0x01,
        SpawnPosition = 0x05,
    }
}
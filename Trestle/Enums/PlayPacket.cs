using Trestle.Attributes;
using Trestle.Networking;

namespace Trestle.Enums
{
    public enum PlayPacket : byte
    {
        // Clientbound
        Client_PlayerPositionAndLook = 0x08,
        Client_ChatMessage = 0x02,
        Client_KeepAlive = 0x00,
        Client_SpawnPosition = 0x05,
        Client_JoinGame = 0x01,
        Client_ChunkData = 0x21,
        
        // Serverbound
        Server_PlayerPositionAndLook = 0x06,
        Server_ChatMessage = 0x01,
        Server_KeepAlive = 0x00,
        Server_Player = 0x03,
        Server_PlayerPosition = 0x04,
        Server_PlayerLook = 0x05
    }
}
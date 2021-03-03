namespace Trestle.Enums
{
    public enum StatusPacket : byte
    {
        // Client-bound
        Request = 0x00,
        Ping = 0x01,
        
        // Server-bound
        Response = 0x00,
        Pong = 0x01
    }
}
namespace Trestle.Enums.Packets.Client
{
    public enum LoginPacket
    {
        Disconnect = 0x00,
        EncryptionRequest = 0x01,
        LoginSuccess = 0x02,
        SetCompression = 0x03
    }
}
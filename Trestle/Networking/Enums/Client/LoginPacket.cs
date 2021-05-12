namespace Trestle.Networking.Enums.Client
{
    public enum LoginPacket : byte
    {
        Disconnect = 0x00,
        EncryptionRequest = 0x01,
        LoginSuccess = 0x02,
        SetCompression = 0x03
    }
}
namespace Trestle.Enums
{
    public enum LoginPacket
    {
        // ServerBound
        LoginStart = 0x00,
        EncryptionResponse = 0x01,
        
        // ClientBound
        Disconnect = 0x40,
        EncryptionRequest = 0x01,
        LoginSuccess = 0x02,
        SetCompression = 0x03,
    }
}
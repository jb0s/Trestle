namespace Trestle.Enums
{
    public enum LoginPacket
    {
        // ServerBound
        LoginStart = 0x00,
        
        // ClientBound
        EncryptionResponse = 0x01,
        LoginSuccess = 0x02,
        SetCompression = 0x03,
    }
}
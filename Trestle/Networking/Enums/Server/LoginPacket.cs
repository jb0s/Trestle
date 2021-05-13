namespace Trestle.Networking.Enums.Server
{
    public enum LoginPacket : byte
    {
        LoginStart = 0x00,
        EncryptionResponse = 0x01,
    }
}
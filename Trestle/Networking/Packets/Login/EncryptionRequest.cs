using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Networking.Packets.Login
{
    [ClientBound(LoginPacket.EncryptionRequest)]
    public class EncryptionRequest : Packet
    {
        [Field]
        public string ServerId { get; set; } = "";

        [Field] 
        [VarInt] 
        public int PublicKeyLength { get; set; } = 0;
        
        [Field]
        public byte[] PublicKey { get; set; }

        [Field] 
        [VarInt] 
        public int VerifyTokenLength { get; set; } = 0;
        
        [Field]
        public byte[] VerifyToken { get; set; }

        public EncryptionRequest(string serverId, byte[] publicKey, byte[] verifyToken)
        {
            ServerId = serverId;
            PublicKeyLength = publicKey.Length;
            PublicKey = publicKey;
            VerifyTokenLength = verifyToken.Length;
            VerifyToken = verifyToken;
        }
    }
}
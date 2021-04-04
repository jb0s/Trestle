using System.Security.Cryptography;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Login.Client
{
    [ClientBound(LoginPacket.EncryptionRequest)]
    public class EncryptionRequest : Packet
    {
        [Field] 
        public string ServerId { get; set; } = "";

        [Field] 
        [VarInt]
        public int PublicKeyLength => PublicKey.Length;
        
        [Field]
        public byte[] PublicKey { get; set; }

        [Field] 
        [VarInt]
        public int VerifyTokenLength => VerifyToken.Length;
        
        [Field]
        public byte[] VerifyToken { get; set; }

        public EncryptionRequest(RSAParameters publicKey, byte[] verifyToken)
        {
            PublicKey = PacketCryptography.PublicKeyToAsn1(publicKey);
            VerifyToken = verifyToken;
        }
    }
}
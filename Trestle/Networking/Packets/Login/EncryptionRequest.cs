using System;
using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Networking.Packets.Login
{
    // Shares ID with EncryptionResponse
    [ClientBound(LoginPacket.EncryptionResponse)]
    public class EncryptionRequest : Packet
    {
        [Field]
        public string ServerId { get; set; } = "";

        [Field] 
        [VarInt] 
        public int PublicKeyLength { get; set; }
        
        [Field]
        public byte[] PublicKey { get; set; }

        [Field] 
        [VarInt] 
        public int VerifyTokenLength { get; set; }
        
        [Field]
        public byte[] VerifyToken { get; set; }

        public EncryptionRequest(byte[] publicKey, byte[] verifyToken)
        {
            PublicKeyLength = publicKey.Length;
            PublicKey = publicKey;
            VerifyTokenLength = verifyToken.Length;
            VerifyToken = verifyToken;
        }
    }
}
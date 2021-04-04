using System;
using System.Linq;
using System.Text;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Networking.Packets.Login.Client;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Login.Server
{
    [ServerBound(LoginPacket.EncryptionResponse)]
    public class EncryptionResponse : Packet
    {
        [Field]
        public byte[] SharedSecret { get; set; }
        
        [Field]
        public byte[] VerifyToken { get; set; }

        public override void HandlePacket()
        {
            Client.SharedKey = PacketCryptography.Decrypt(SharedSecret);

            var recv = PacketCryptography.GenerateAes((byte[])Client.SharedKey.Clone());
            var send = PacketCryptography.GenerateAes((byte[])Client.SharedKey.Clone());
            
            var token = PacketCryptography.Decrypt(VerifyToken);

            // Checks if the verify token is the same.
            if (!token.SequenceEqual(PacketCryptography.VerifyToken))
            {
                Client.SendPacket(new Disconnect(new MessageComponent("Authentication failed: wrong token! ")));
                return;
            }

            // Creates the Encryptor & Decryptor for packets.
            Client.Decrypter = recv.CreateDecryptor();
            Client.Encrypter = recv.CreateDecryptor();

            var userJoined = Mojang.HasJoined(Client);
            if (userJoined == null)
            {
                Client.SendPacket(new Disconnect(new MessageComponent("Authentication failed!")));
                return;
            }

            Client.SendPacket(new SetCompression(Config.UseCompression ? Config.CompressionThreshold : -1));
            Client.SendPacket(new LoginSuccess(userJoined.Id, userJoined.Name));

            Client.CreatePlayer(userJoined.Id, userJoined.Name);
        }
    }
}
using System;
using System.Net;
using System.Linq;
using Trestle.Utils;
using Trestle.Enums;
using Trestle.Entity;
using Trestle.Attributes;
using Trestle.Networking.Packets.Play;
using Trestle.Networking.Packets.Play.Client;

namespace Trestle.Networking.Packets.Login
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

            var packetToken = PacketCryptography.Decrypt(VerifyToken);

            if (!packetToken.SequenceEqual(PacketCryptography.VerifyToken))
            {
                Client.SendPacket(new LoginDisconnect(new MessageComponent("Authentication failed: wrong token! :(")));
                return;
            }

            Client.Decrypter = recv.CreateDecryptor();
            Client.Encrypter = send.CreateEncryptor();
            Client.EncryptionEnabled = true;

            Client.SendPacket(new LoginSuccess(Client.Username, Guid.NewGuid()));
            
            Client.State = ClientState.Play;
            
            Client.Player = new Player(-1, Globals.WorldManager.MainWorld)
            {
                Uuid = GetUuid(Client.Username),
                Username = Client.Username,
                Client = Client,
                GameMode = Globals.WorldManager.MainWorld.DefaultGameMode
            };

            if (Client.Player.IsAuthenticated)
            {
                Client.SendPacket(new JoinGame(Client));
                Client.Player.InitializePlayer();
                Client.Player.SendChunksForLocation(true);
            }
            else
            {
                Client.SendPacket(new Disconnect(new MessageComponent("Authentication failed!")));
            }
        }
        
        private string GetUuid(string username)
        {
            try
            {
                var wc = new WebClient();
                var result = wc.DownloadString("https://api.mojang.com/users/profiles/minecraft/" + username);
                var resultSplit = result.Split('"');
                
                if (resultSplit.Length > 1)
                {
                    var uuid = resultSplit[7];
                    return new Guid(uuid).ToString();
                }
                
                return Guid.NewGuid().ToString();
            }
            catch(Exception e)
            {
                Client.SendPacket(new LoginDisconnect(new MessageComponent(e.Message)));
                return "";
            }
        }
    }
}
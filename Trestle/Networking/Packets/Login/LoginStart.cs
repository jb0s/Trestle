using System;
using System.Net;
using System.Text;
using Trestle.Attributes;
using Trestle.Entity;
using Trestle.Enums;
using Trestle.Networking.Packets.Play;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Login
{
    [ServerBound(LoginPacket.LoginStart)]
    public class LoginStart : Packet
    {
        [Field]
        public string Name { get; set; }
        
        public override void HandlePacket()
        {
            var uuid = GetUuid(Name);
            
            if (Config.OnlineMode)
            {
                if (Config.EncryptionEnabled)
                {
                    Client.State = ClientState.Login;
                    Client.Username = Name;
                    Client.SendPacket(new EncryptionRequest("", PacketCryptography.PublicKeyToAsn1(Globals.ServerKey), PacketCryptography.GetRandomToken()));
                }

                if (!Client.Player.IsAuthenticated)
                {
                    
                }
            }

            if (Encoding.UTF8.GetBytes(Name).Length == 0)
            {
                
            }

            if (Client.Protocol < Globals.ProtocolVersion)
            {
                
            }

            if (Client.Protocol > Globals.ProtocolVersion)
            {
                
            }
            
            Client.SendPacket(new SetCompression(Config.UseCompression ? Config.CompressionThreshold : -1));
            Client.SendPacket(new LoginSuccess(Name, Guid.NewGuid()));
            
            Client.Player = new Player(-1, Globals.WorldManager.MainWorld)
            {
                UUID = uuid,
                Username = Name,
                Client = Client,
                GameMode = Globals.WorldManager.MainWorld.DefaultGameMode
            };

            Client.State = ClientState.Play;

            Client.SendPacket(new JoinGame(Client));
            Client.SendPacket(new SpawnPosition());
            Client.SendPacket(new PlayerPositionAndLook(Client, new Location(0, 0, 0)));
            
            Client.Player.InitializePlayer();
            Client.Player.SendChunksForLocation();
        }
        
        private string GetUuid(string username)
        {
            try
            {
                var wc = new WebClient();
                var result = wc.DownloadString("https://api.mojang.com/users/profiles/minecraft/" + username);
                var _result = result.Split('"');
                if (_result.Length > 1)
                {
                    var uuid = _result[3];
                    return new Guid(uuid).ToString();
                }
                return Guid.NewGuid().ToString();
            }
            catch
            {
                return Guid.NewGuid().ToString();
            }
        }
    }
}
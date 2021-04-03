using System;
using System.Linq;
using System.Net;
using System.Text;
using Trestle.Attributes;
using Trestle.Entity;
using Trestle.Enums;
using Trestle.Networking.Packets.Play;
using Trestle.Networking.Packets.Play.Client;
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
            var username = new string(Name.Where(c => char.IsLetter(c) || char.IsPunctuation(c) || char.IsDigit(c)).ToArray());
            var uuid = GetUuid(username);
            
            Client.State = ClientState.Login;
            Client.Username = username;

            if (Encoding.UTF8.GetBytes(Name).Length == 0)
                Client.SendPacket(new LoginDisconnect(new MessageComponent("Authentication failed!")));

            if (Client.Protocol < Globals.ProtocolVersion)
                Client.SendPacket(new LoginDisconnect(new MessageComponent($"Client too old! I'm on {Globals.OfficialProtocolName}")));

            if (Client.Protocol > Globals.ProtocolVersion)
                Client.SendPacket(new LoginDisconnect(new MessageComponent($"Client too new! I'm still on {Globals.OfficialProtocolName}")));
            
            // Encryption
            if (Config.OnlineMode && Config.EncryptionEnabled)
            {
                Client.SendPacket(new EncryptionRequest(PacketCryptography.PublicKeyToAsn1(Globals.ServerKey), PacketCryptography.GetRandomToken()));
                return;
            }
            
            Client.SendPacket(new SetCompression(Config.UseCompression ? Config.CompressionThreshold : -1));
            Client.SendPacket(new LoginSuccess(Name, Guid.NewGuid()));
            
            Client.Player = new Player(-1, Globals.WorldManager.MainWorld)
            {
                Uuid = uuid,
                Username = Name,
                Client = Client,
                GameMode = Globals.WorldManager.MainWorld.DefaultGameMode
            };

            Client.State = ClientState.Play;

            // Check if authentication went well
            if (Config.OnlineMode && !Client.Player.IsAuthenticated)
            {
                Client.SendPacket(new Disconnect(new MessageComponent("Authentication failed!")));
                return;
            }
            
            Client.SendPacket(new JoinGame(Client));
            Client.Player.InitializePlayer();
            Client.Player.SendChunksForLocation(true);
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
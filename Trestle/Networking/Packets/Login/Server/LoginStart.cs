using System;
using System.Linq;
using System.Text;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Server;
using Trestle.Networking.Packets.Login.Client;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Login.Server
{
    [ServerBound(LoginPacket.LoginStart)]
    public class LoginStart : Packet
    {
        [Field]
        public string Name { get; set; }

        public override void HandlePacket()
        {
            var username = new string(Name.Where(c => char.IsLetter(c) || char.IsPunctuation(c) || char.IsDigit(c)).ToArray());
            var uuid = Mojang.GetProfileByUsername(username)?.Id;

            Client.Username = username;

            if (uuid == null)
                uuid = Guid.NewGuid().ToString("D");
            else
                uuid = Guid.Parse(uuid).ToString("D");

            // If an already online player with the same uuid exists, kick them first.
            TrestleServer.GetOnlinePlayers().ToList().Find(player => player.Uuid == uuid)?.Kick(new MessageComponent("You've logged in from another location"));

            // Checks if the server is online mode & isn't localhost, and then starts the encryption process.
            if (Config.OnlineMode && !Client.TcpClient.Client.RemoteEndPoint.ToString().Contains("127.0.0.1"))
            {
                Client.SendPacket(new EncryptionRequest(Globals.ServerKey, PacketCryptography.GetRandomToken()));
                return;
            }
            
            Client.SendPacket(new SetCompression(Config.UseCompression ? Config.CompressionThreshold : -1));
            Client.SendPacket(new LoginSuccess(uuid, username));
            
            Client.CreatePlayer(uuid, username);
        }
    }
}
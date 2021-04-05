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

            Client.Username = username;

            // Checks if the server is online mode & isn't localhost, and then starts the encryption process.
            if (Config.OnlineMode && !Client.TcpClient.Client.RemoteEndPoint.ToString().Contains("127.0.0.1"))
            {
                Client.SendPacket(new EncryptionRequest(Globals.ServerKey, PacketCryptography.GetRandomToken()));
                return;
            }

            var uuid = Mojang.GetProfileByUsername(username)?.Id;
            if (uuid == null)
                uuid = Guid.NewGuid().ToString("D");
            else
                uuid = Guid.Parse(uuid).ToString("D");
            
            Client.SendPacket(new SetCompression(Config.UseCompression ? Config.CompressionThreshold : -1));
            Client.SendPacket(new LoginSuccess(uuid, username));
            
            Client.CreatePlayer(uuid, username);
        }
    }
}
using System;
using System.Threading.Tasks;
using Trestle.Networking.Attributes;
using Trestle.Networking.Enums.Server;
using Trestle.Networking.Packets.Login.Client;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Login.Server
{
    [ServerBound(LoginPacket.LoginStart)]
    public class LoginStart : Packet
    {
        [Field]
        public string Name { get; set; }

        public override async Task Handle()
        {
            // TODO: add encryption and online mode.
            if (false && !Client.IsLocalhost)
            { }
            
            // NOTE: in notchian offline mode, it doesn't use a names actual uuid, but that isn't fun :(
            var uuid = MojangService.GetUuid(Name);
            if (uuid == Uuid.Empty)
                uuid = Uuid.NewUuid();
            
            Client.SendPacket(new LoginSuccess(uuid, Name));
        }
    }
}
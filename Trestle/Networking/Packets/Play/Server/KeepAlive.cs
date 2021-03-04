using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.Server_KeepAlive)]
    public class KeepAlive : Packet
    {
        [Field]
        public long KeepAliveId { get; set; }

        public override void HandlePacket()
        {
            Logger.Debug($"Successfully kept alive {Client.Player.Username} with msid {KeepAliveId}! Previously missed {Client.MissedKeepAlives - 1} KeepAlives.");
            Client.MissedKeepAlives = 0;
        }
    }
}
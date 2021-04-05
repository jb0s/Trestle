using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Server;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.KeepAlive)]
    public class KeepAlive : Packet
    {
        [Field]
        [VarInt]
        public long KeepAliveId { get; set; }

        public override void HandlePacket()
        {
            // this log annoys me
            //Logger.Debug($"Successfully kept alive {Client.Player.Username} with msid {KeepAliveId}! Previously missed {Client.MissedKeepAlives - 1} KeepAlives.");
            Client.MissedKeepAlives = 0;
        }
    }
}
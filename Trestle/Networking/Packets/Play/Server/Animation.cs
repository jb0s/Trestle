using System;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Server;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.Animation)]
    public class Animation : Packet
    {
        [Field]
        [VarInt]
        public int Hand { get; set; }

        public override void HandlePacket()
        {
            Client.Player.World.BroadcastPacket(new Client.Animation(Client.Player, AnimationType.SwingArm), Client.Player);
        }
    }
}
using System;
using System.Numerics;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Server;
using Trestle.Networking.Packets.Play.Client;
using Trestle.Utils;
using Vector3 = Trestle.Utils.Vector3;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.UseEntity)]
    public class UseEntity : Packet
    {
        [Field]
        [VarInt]
        public int Target { get; set; }
        
        [Field(typeof(int))]
        [VarInt]
        public InteractionType Type { get; set; }

        public override void HandlePacket()
        {
            switch (Type)
            {
                // TODO: Make this not bad
                case InteractionType.Attack:
                    if (World.Players.ContainsKey(Target))
                    {
                        var player = Player.World.Players[Target];
                        if (player.HealthManager.Pain(1))
                        {
                            double xRatio = Player.Location.X - player.Location.X;
                            double zRatio;
                        
                            for (zRatio = player.Location.Z - Player.Location.Z; xRatio * xRatio + zRatio * zRatio < 1.0E-4d; zRatio = (Globals.Random.Next() - Globals.Random.Next()) * 0.01d)
                                xRatio = (Globals.Random.Next() - Globals.Random.Next()) * 0.01d;
                        
                            float attackedAtYaw = (float)(Math.Atan2(zRatio, xRatio) * (180d / Math.PI) - (double)player.Location.Yaw);
                            player.Knockback(Player, 0.4f, xRatio, zRatio);
                        }
                    }
                    else if(World.Entities.ContainsKey(Target))
                        Player.World.Entities[Target].HealthManager.Pain(1);
                    break;
            }
        }
    }
}
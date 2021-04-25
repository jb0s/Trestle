using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Server;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.ClientStatus)]
    public class ClientStatus : Packet
    {
        [Field(typeof(int))]
        [VarInt]
        public ActionType ActionType { get; set; }

        public override void HandlePacket()
        {
            switch (ActionType)
            {
                case ActionType.PerformRespawn:
                    Client.Player.Respawn();
                    break;
                
                case ActionType.RequestStats:
                    Client.Player.SendChat("Sorry, this isn't implemented yet!");
                    break;
            }
        }
    }
}
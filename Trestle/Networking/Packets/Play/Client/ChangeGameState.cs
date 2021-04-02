using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.Client_ChangeGameState)]
    public class ChangeGameState : Packet
    {
        [Field] 
        public byte Reason;

        [Field] 
        public float Value;

        public ChangeGameState(GameStateReason reason, float value = 0f)
        {
            Reason = (byte)reason;
            Value = value;
        }
    }
}
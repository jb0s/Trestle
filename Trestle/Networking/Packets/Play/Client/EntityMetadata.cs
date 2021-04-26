using System;
using Trestle.Enums;
using Trestle.Entity;
using Trestle.Attributes;
using System.Collections.Generic;
using Trestle.Enums.Packets.Client;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.EntityMetadata)]
    public class EntityMetadata : Packet
    {
        [Field]
        [VarInt]
        public int EntityId { get; set; }
        
        [Field]
        public byte[] Data { get; set; }

        public EntityMetadata(Entity.Entity entity)
        {
            EntityId = entity.EntityId;
            Data = entity.Metadata.ToArray();
        }
    }
}
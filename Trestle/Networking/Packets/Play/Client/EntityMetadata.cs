using System;
using System.Collections.Generic;
using Trestle.Attributes;
using Trestle.Entity;
using Trestle.Enums;
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
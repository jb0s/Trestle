using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Trestle.Worlds;
using Trestle.Entity;
using Trestle.Enums;
using Trestle.Items;
using Trestle.Networking;
using Trestle.Networking.Packets;
using Trestle.Networking.Packets.Play.Client;
using Trestle.Utils;
using Trestle.Worlds.Structures;

namespace Trestle.Worlds
{
    public class World
    {
        public string Name { get; set; }
        public int Dimension { get; set; }
        public int Difficulty { get; set; }
        public WorldType WorldType { get; set; }
        
        public int Day { get; set; }
        public bool Raining { get; set; }
        public int TimeToRain { get; set; }
        public int CurrentWorldTime { get; set; }
        public GameMode DefaultGameMode { get; set; }
        
        public WorldGenerator Generator { get; set; }
        public List<Entity.Entity> Entities { get; private set; }
        public Dictionary<int, Player> OnlinePlayers { get; private set; }
        public Dictionary<Vector3, int> BlockWithTicks { get; private set; }
        public Dictionary<string, List<ForcedBlock>> BlocksToForce { get; set; }
        public Player[] OnlinePlayerArray => OnlinePlayers.Values.ToArray();

        public World()
        {
            CurrentWorldTime = 1200;
            Day = 1;
            Dimension = 0;
            TimeToRain = Globals.Random.Next(10000, 98000);
            Generator = new WorldGenerator(this);

            Entities = new List<Entity.Entity>();
            OnlinePlayers = new Dictionary<int, Player>();
            BlockWithTicks = new Dictionary<Vector3, int>();
            BlocksToForce = new Dictionary<string, List<ForcedBlock>>();
        }
        
        public void RemovePlayer(Player player)
        {
            RemovePlayer(player.EntityId);
        }
        
        public void RemovePlayer(int entityId)
        {
            lock (OnlinePlayers)
            {
                if (OnlinePlayers.ContainsKey(entityId))
                {
	                OnlinePlayers.Remove(entityId);
                }
            }
        }

        public Player GetPlayer(int entityId)
        {
            if (OnlinePlayers.ContainsKey(entityId))
            {
                return OnlinePlayers[entityId];
            }

            return null;
        }
        
        public void AddPlayer(Player player)
        {
            OnlinePlayers.Add(player.EntityId, player);

            // TODO: Find new packet
            /*new PlayerListItem(player.Wrapper)
            {
                Action = 0,
                GameMode = player.GameMode,
                Username = player.Username,
                Uuid = player.UUID
            }.Broadcast(this);*/

            IntroduceNewPlayer(player.Client);
        }

        public void BroadcastChat(MessageComponent message)
        {
            BroadcastChat(message, ChatMessageType.ChatBox, null);
        }
        
        public void BroadcastChat(MessageComponent message, Player sender)
        {
            BroadcastChat(message, ChatMessageType.ChatBox, sender);
        }

        public void BroadcastChat(MessageComponent message, ChatMessageType messagetype, Player sender)
        {
            foreach (var i in OnlinePlayers.Values)
            {
                if (i == sender)
                {
                    continue;
                }
                
                i.Client.SendPacket(new ChatMessage(message));
            }
        }
        
        internal void IntroduceNewPlayer(Client caller)
        {
            foreach (var i in OnlinePlayers.Values)
            {
	            // TODO: Find new packet
	            /*new PlayerListItem(caller)
	            {
		            Action = 0,
		            GameMode = i.GameMode,
		            Username = i.Username,
		            Uuid = i.UUID
	            }.Write();*/
	            
                if (i.Client != caller)
                {
					//new SpawnPlayer(caller) {Player = i}.Write();
	               // new SpawnPlayer(i.Wrapper) {Player = caller.Player}.Write();
                    i.BroadcastInventory();
                }
            }
        }
        
        internal void BroadcastPlayerRemoval(Client caller)
        {
	        // TODO: Find new packet
            /*new PlayerListItem(caller)
            {
                Action = 4,
                Uuid = caller.Player.UUID
            }.Broadcast(this);*/
        }
        
        public void SaveChunks()
            => Generator.SaveChunks(Name);

        private int Mod(double val)
            => (int)(((val%16) + 16)%16);
        
        public Block GetBlock(Vector3 blockCoordinates)
		{
			Vector2 chunkcoords = new Vector2((int) blockCoordinates.X >> 4, (int) blockCoordinates.Z >> 4);
			var chunk = Generator.GenerateChunk(chunkcoords);

			var bid = chunk.GetBlock(Mod(blockCoordinates.X), (int) blockCoordinates.Y, Mod(blockCoordinates.Z));

			var metadata = chunk.GetMetadata(Mod(blockCoordinates.X), (int)blockCoordinates.Y,
				Mod(blockCoordinates.Z));

			var block = new Block(ItemFactory.GetItemById(bid, metadata));
			block.Coordinates = blockCoordinates;
			block.Metadata = metadata;

			return block;
		}

        // TODO: Add this
		/*public void UpdateSign(Vector3 coordinates, string[] lines)
		{
			if (lines.Length >= 4)
			{
				var signUpdate = new UpdateSign(null)
				{
					SignCoordinates = coordinates,
					Line1 = lines[0],
					Line2 = lines[1],
					Line3 = lines[2],
					Line4 = lines[4],
				};
				BroadcastPacket(signUpdate);
			}
		}*/

		public void SetBlock(Vector3 coordinates, Block block)
		{
			block.Coordinates = coordinates;
			SetBlock(block);
		}

		public void SetBlock(Block block, bool broadcast = true, bool applyPhysics = true)
		{
			var chunk = Generator.GenerateChunk(new Vector2((int) block.Coordinates.X >> 4, (int) block.Coordinates.Z >> 4));
			
			chunk.SetBlock(Mod(block.Coordinates.X), (int)block.Coordinates.Y,
				Mod(block.Coordinates.Z),
				block);
			chunk.IsDirty = true;

			if (applyPhysics) ApplyPhysics((int) block.Coordinates.X, (int) block.Coordinates.Y, (int) block.Coordinates.Z);

			if (!broadcast) return;
			
			/*
			var packet = new BlockChange(null)
			{
				BlockId = block.Id,
				Metadata = block.Metadata,
				Location = block.Coordinates
			};*/
			//BroadcastPacket(packet);
		}

		internal void ApplyPhysics(int x, int y, int z)
		{
			DoPhysics(x - 1, y, z);
			DoPhysics(x + 1, y, z);
			DoPhysics(x, y - 1, z);
			DoPhysics(x, y + 1, z);
			DoPhysics(x, y, z - 1);
			DoPhysics(x, y, z + 1);
			DoPhysics(x, y, z);
		}

		private void DoPhysics(int x, int y, int z)
		{
			var block = GetBlock(new Vector3(x, y, z));
			if (block.Material == Material.Air) return;
			block.DoPhysics(this);
		}

		public void ScheduleBlockTick(Block block, int tickRate)
		{
			BlockWithTicks[block.Coordinates] = CurrentWorldTime + tickRate;
		}
        
		public void AddEntity(Entity.Entity entity)
		{
			Entities.Add(entity);
		}

		public void RemoveEntity(Entity.Entity entity)
		{
			if (Entities.Contains(entity)) Entities.Remove(entity);
		}

        public void BroadcastPacket(Packet packet)
            => BroadcastPacket(packet, true);
        
        public void BroadcastPacket(Packet packet, bool self)
        {
            foreach (var i in OnlinePlayers.Values)
            {
                if (i == null) continue;
                if (!self && packet.Client != null && i.Client.ClientIdentifier == packet.Client.ClientIdentifier) continue;
               // packet.SetTarget(i.Wrapper);
                //packet.Write();
            }
        }

        public Location GetSpawnPoint()
            => Generator.GetSpawnPoint().ToLocation();
    }
}
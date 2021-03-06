using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using Trestle.Entity.Tile;
using Trestle.Enums;
using Trestle.Networking;
using Trestle.Networking.Packets;
using Trestle.Networking.Packets.Play;
using Trestle.Utils;
using Trestle.Worlds;
using Animation = Trestle.Enums.Animation;
using ObjectType = Trestle.Enums.ObjectType;
using Vector3 = Trestle.Utils.Vector3;

namespace Trestle.Entity
{
    public class Player : Entity
    {
        /// <summary>
        /// The chunks this entity has loaded.
        /// </summary>
        private readonly Dictionary<Tuple<int, int>, ChunkColumn> _chunksUsed;
        
        /// <summary>
        /// The position of the current chunk the player is in.
        /// </summary>
        private Vector2 _currentChunkPosition = new Vector2(0, 0);
        
        /// <summary>
        /// The player's inventory.
        /// </summary>
        public InventoryManager Inventory;
        
        /// <summary>
        /// The player's skin blob.
        /// </summary>
        public string SkinBlob = "";

        /// <summary>
		/// The player's username
		/// </summary>
		public string Username { get; set; }
		
		/// <summary>
		/// The player's UUID
		/// </summary>
		public string UUID { get; set; }
		
		/// <summary>
		/// The player's associated ClientWrapper
		/// </summary>
		public Client Client { get; set; }
		
		/// <summary>
		/// The player's gamemode
		/// </summary>
		public GameMode GameMode { get; set; } 
		
		/// <summary>
		/// Has the player spawned?
		/// </summary>
		public bool HasSpawned { get; set; } 
		
		/// <summary>
		/// Is the player digging?
		/// </summary>
		public bool Digging { get; set; }
		
		/// <summary>
		/// Can the player fly?
		/// </summary>
		private bool CanFly { get; set; }
		
		/// <summary>
		/// The locale of the client.
		/// </summary>
		public string Locale { get; set; }
		
		/// <summary>
		/// The player's view distance.
		/// </summary>
		public byte ViewDistance { get; set; }
		
		/// <summary>
		/// Any chat flags the player may have set.
		/// </summary>
		public int ChatFlags { get; set; }
		
		/// <summary>
		/// Enable chat colours?
		/// </summary>
		public bool ChatColours { get; set; }
		
		/// <summary>
		/// Player's skin parts.
		/// </summary>
		public byte SkinParts { get; set; }
		
		/// <summary>
		/// Player's main hand.
		/// </summary>
		public int MainHand { get; set; }
		
		/// <summary>
		/// Force a chunk reload next tick?
		/// </summary>
		public bool ForceChunkReload { get; set; }
		
		/// <summary>
		/// Last action the entity performed.
		/// </summary>
		public EntityAction LastEntityAction { get; set; }
		
		/// <summary>
		/// Is the player an operator?
		/// </summary>
		public bool IsOperator { get; internal set; }
		
		/// <summary>
		/// Is the player loaded?
		/// </summary>
		private bool IsLoaded { get; set; }
		
		/// <summary>
		/// Is the player crouching?
		/// </summary>
		public bool IsCrouching { get; set; }
		
		/// <summary>
		/// Is the player authenticated with Mojang?
		/// </summary>
		/// <returns></returns>
		public bool IsAuthenticated
		{
			get
			{
				if (Config.OnlineMode)
				{
					try
					{
						var uri = new Uri(
							string.Format(
								"http://session.minecraft.net/game/checkserver.jsp?user={0}&serverId={1}",
								Username,
								PacketCryptography.JavaHexDigest(Encoding.UTF8.GetBytes("")
									.Concat(Client.SharedKey)
									.Concat(PacketCryptography.PublicKeyToAsn1(Globals.ServerKey))
									.ToArray())
							));

						var authenticated = new WebClient().DownloadString(uri);
						if (authenticated.Contains("NO"))
						{
							Console.WriteLine("Player authenticated: " + authenticated);
							return false;
						}
					}
					catch
					{
						return false;
					}

					return true;
				}

				return true;
			}
		}
        
        public Player(int entityTypeId, World world) : base(-1, world)
        {
	        _chunksUsed = new Dictionary<Tuple<int, int>, ChunkColumn>();
            Inventory = new InventoryManager(this);
            World = world;

            // TODO: Fix this dumb workaround
            EntityId = Globals.Random.Next(0, 999999999);
            
            // TODO: Unhardcode viewdistance
            ViewDistance = 16;
            
            IsOperator = false;
            IsLoaded = false;
        }
        
        public override void OnTick()
        {
	        if (HasSpawned)
	        {
		        if (GameMode == GameMode.Survival)
		        {
			        //HealthManager.OnTick();
		        }
	        }
        }
        
        /// <summary>
        /// Handler for when the client moves around the world.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="yaw"></param>
        /// <param name="pitch"></param>
        /// <param name="onGround"></param>
        public void PositionChanged(Vector3 location, float yaw = 0.0f, float pitch = 0.0f, bool onGround = false)
        {
	        var originalchunkcoords = new Vector2(_currentChunkPosition.X, _currentChunkPosition.Z);
	        var originalcoordinates = Location;
	        if (yaw != 0.0f && pitch != 0.0f)
	        {
		        Location.Yaw = yaw;
		        Location.Pitch = pitch;
	        }
	        Location.Y = location.Y;
	        Location.X = location.X;
	        Location.Z = location.Z;
	        Location.OnGround = onGround;

	        _currentChunkPosition.X = (int) location.X >> 4;
	        _currentChunkPosition.Z = (int) location.Z >> 4;

	        //if (originalchunkcoords != _currentChunkPosition) 
		        SendChunksForLocation();
		        
	        /*
	        new EntityTeleport(Wrapper)
	        {
		        EntityId = EntityId,
		        Coordinates = location,
		        OnGround = onGround,
		        Pitch = (byte) pitch,
		        Yaw = (byte) yaw,
	        }.Broadcast(World, false, this);*/

	        LookChanged();
        }
        
        /// <summary>
        /// Handler for when the client changes look direction.
        /// </summary>
        public void LookChanged()
        {
	        /*
	        new EntityLook(Wrapper)
	        {
		        EntityId = this.EntityId,
		        Pitch = Location.Pitch,
		        Yaw = Location.Yaw,
		        OnGround = Location.OnGround
	        }.Broadcast(World, false, this);
			
	        new EntityHeadLook(Wrapper)
	        {
		        EntityId = this.EntityId,
		        HeadYaw = Location.Yaw,
	        }.Broadcast(World, false, this);*/
        }
        
        /// <summary>
		/// Handler for when the client changes the held item.
		/// </summary>
		/// <param name="slot"></param>
		public void HeldItemChanged(int slot)
		{
			Inventory.HeldItemChanged(slot);
			BroadcastInventory();
		}

		/// <summary>
		/// Makes the player swing their hand.
		/// </summary>
		/// <param name="hand"></param>
		public void PlayerHandSwing(byte hand)
		{
			//PlayerAnimation(Animation.SwingArm, hand);
		}

		/// <summary>
		/// Performs an animation on the client.
		/// </summary>
		/// <param name="animation"></param>
		/// <param name="hand"></param>
		public void PlayerAnimation(Animation animation, byte hand = 0)
		{
			/*
			var packet = new Networking.Packets.Animation(Wrapper) { EntityId = EntityId, AnimationId = (byte)animation, Hand = hand};
			World.BroadcastPacket(packet, false);*/
		}

		/// <summary>
		/// Sends the player's inventory to the client.
		/// </summary>
		public void BroadcastInventory()
		{
			/*
			// Main Hand
			var slotdata = Inventory.GetSlot(36 + Inventory.CurrentSlot);
			new EntityEquipment(Wrapper)
			{
				Slot = EquipmentSlot.Hand0,
				Item = slotdata,
				EntityId = EntityId
			}.Broadcast(World, false, this);

			// Second hand
			slotdata = Inventory.GetSlot(45);
			new EntityEquipment(Wrapper)
			{
				Slot = EquipmentSlot.Hand1,
				Item = slotdata,
				EntityId = EntityId
			}.Broadcast(World, false, this);

			// Helmet
			slotdata = Inventory.GetSlot(5);
			new EntityEquipment(Wrapper)
			{
				Slot = EquipmentSlot.Helmet,
				Item = slotdata,
				EntityId = EntityId
			}.Broadcast(World, false, this);

			// Chestplate
			slotdata = Inventory.GetSlot(6);
			new EntityEquipment(Wrapper)
			{
				Slot = EquipmentSlot.Chestplate,
				Item = slotdata,
				EntityId = EntityId
			}.Broadcast(World, false, this);

			// Leggings
			slotdata = Inventory.GetSlot(7);
			new EntityEquipment(Wrapper)
			{
				Slot = EquipmentSlot.Leggings,
				Item = slotdata,
				EntityId = EntityId
			}.Broadcast(World, false, this);

			// Boots
			slotdata = Inventory.GetSlot(8);
			new EntityEquipment(Wrapper)
			{
				Slot = EquipmentSlot.Boots,
				Item = slotdata,
				EntityId = EntityId
			}.Broadcast(World, false, this);*/
		}
		
		/// <summary>
		/// Silently sets the player's game mode.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="silent"></param>
		public void SetGamemode(GameMode target, bool silent)
		{
			GameMode = target;
			
			// TODO: I'm not sure what this packet is now, it's changed.
			/*new PlayerListItem(Wrapper)
			{
				Action = 1,
				GameMode = GameMode,
				Uuid = UUID
			}.Broadcast(World);*/

			/*
			new ChangeGameState(Wrapper)
			{
				Reason = GameStateReason.ChangeGameMode,
				Value = (float) target
			}.Write();*/

			if (!silent)
			{
				Console.WriteLine(Username + "'s gamemode was changed to " + target.ToString("D"));
				SendChat("Your gamemode was changed to " + target.ToString());
			}
		}

		/// <summary>
		/// Sets the player's game mode.
		/// </summary>
		/// <param name="target"></param>
		public void SetGamemode(GameMode target)
		{
			SetGamemode(target, false);
		}
		
		/// <summary>
		/// Teleports the player entity.
		/// </summary>
		/// <param name="newPosition"></param>
		public void Teleport(Location newPosition)
		{
			/*
			new EntityTeleport(Wrapper)
			{
				EntityId = EntityId,
				Coordinates = newPosition.ToVector3(),
				OnGround = newPosition.OnGround,
				Pitch = newPosition.Pitch,
				Yaw = newPosition.Yaw
			}.Broadcast(World, true, this);*/
		}

		/// <summary>
		/// Respawns the player entity.
		/// </summary>
		public void Respawn()
		{
			// TODO: Health
			//HealthManager.ResetHealth();
			//if (Wrapper != null && Wrapper.TcpClient.Connected)
			//{
				//new Respawn(Wrapper) {GameMode = (byte) GameMode}.Write();
				//Teleport(Level.GetSpawnPoint());
			//}
		}
		
		/// <summary>
		/// Send the player's health to the client.
		/// </summary>
		public void SendHealth()
		{
			// TODO: Health
			//new UpdateHealth(Wrapper).Write();
		}

		/// <summary>
		/// Initializes the Player entity.
		/// </summary>
		internal void InitializePlayer()
		{
			if (!IsLoaded)
			{
				LoadPlayer();
				string savename = Config.OnlineMode ? UUID : Username;
				//IsOperator = OperatorLoader.IsOperator(savename);
			}

			Client.SendPacket(new PlayerPositionAndLook(Client, Location));

			HasSpawned = true;
			World.AddPlayer(this);
			Client.Player.Inventory.SendToPlayer();
			BroadcastInventory();
			SetGamemode(GameMode, true);
		}
		
		/// <summary>
		/// Send cached chunk for position.
		/// </summary>
		/// <param name="force"></param>
		public void SendChunksForLocation(bool force = false)
		{
			var centerX = (int) Location.X >> 4;
			var centerZ = (int) Location.Z >> 4;

			if (!force && HasSpawned && _currentChunkPosition == new Vector2(centerX, centerZ)) 
				return;

			_currentChunkPosition.X = centerX;
			_currentChunkPosition.Z = centerZ;
			
			Client.ThreadPool.LaunchThread(() =>
			{
				
				//null, new ChunkLocation(SpawnPoint), new Dictionary<Tuple<int, int>, byte[]>(), 8)
				//ViewDistance, force ? new List<Tuple<int, int>>() : _chunksUsed, this)
				foreach (var chunk in World.GenerateChunks(this, new ChunkLocation(Location), force ? _chunksUsed : new Dictionary<Tuple<int, int>, ChunkColumn>(), ViewDistance))
				{
					if (Client != null && Client.TcpClient.Connected)
						Client.SendPacket(new ChunkData(chunk));
				}
			});
		}

		/// <summary>
		/// Gets the closest chunk to the player.
		/// </summary>
		public ChunkColumn GetChunk()
		{
			int chunkX = (int)Location.X / 16;
			int chunkZ = (int)Location.Z / 16;
			ChunkColumn chunk = World.WorldGenerator.GenerateChunk(new ChunkLocation(chunkX, chunkZ));
			return chunk;
		}
		
		/// <summary>
		/// Get all entities in a specific chunk.
		/// </summary>
		/// <param name="chunkX"></param>
		/// <param name="chunkZ"></param>
		public void GetEntitysInChunk(int chunkX, int chunkZ)
		{
			foreach (var player in World.Players.Values.ToArray())
			{
				if (player == this) continue;

				var x = (int)player.Location.X >> 4;
				var z = (int)player.Location.Z >> 4;
				if (chunkX == x && chunkZ == z)
				{
					//new SpawnPlayer(Client){ Player = player }.Write();
				}
			}

			foreach (var entity in World.Entities.Values.ToArray())
			{
				var x = (int)entity.Location.X >> 4;
				var z = (int)entity.Location.Z >> 4;
				if (chunkX == x && chunkZ == z)
				{
					//new SpawnObject(Wrapper){X = entity.Location.X, Y = entity.Location.Y, Z = entity.Location.Z, EntityId = entity.EntityId, Type = (ObjectType)entity.EntityTypeId, Yaw = entity.Location.Yaw, Pitch = entity.Location.Pitch}.Write();
				}
			}

			ChunkColumn chunk = World.WorldGenerator.GenerateChunk(new ChunkLocation(chunkX, chunkZ));
			// TODO: Re-add this
			/*foreach (var raw in chunk.TileEntities)
			{
				var nbt = raw.Value;
				if (nbt == null) continue;

				string id = null;
				var idTag = nbt.Get("id");
				if (idTag != null)
				{
					id = idTag.StringValue;
				}

				if (string.IsNullOrEmpty(id)) continue;

				var tileEntity = TileEntityFactory.GetBlockEntityById(id);
				tileEntity.Coordinates = raw.Key;
				tileEntity.SetCompound(nbt);

				if (tileEntity.Id == "Sign")
				{
					throw new NotImplementedException();
					var sign = (SignTileEntity) tileEntity;
					new UpdateSign(Wrapper)
					{
						SignCoordinates = sign.Coordinates,
						Line1 = sign.Line1,
						Line2 = sign.Line2,
						Line3 = sign.Line3,
						Line4 = sign.Line4,
					}.Write();
				}
			}*/
		}
		
		/// <summary>
		/// Unload the chunk at X, Y
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		internal void UnloadChunk(int x, int y)
		{
			/*
			new ChunkData(Wrapper)
			{
				Queue = false,
				Unloader = true,
				Chunk = new Chunk(World, x, y, new List<ForcedBlock>())
			}.Write();*/
		}
		
		/// <summary>
		/// Send a chat message.
		/// </summary>
		/// <param name="message"></param>
		public void SendChat(MessageComponent message)
		{
			/*
			if (Wrapper.TcpClient == null)
			{
				Console.WriteLine(message.Text);
				return;
			}

			new ChatMessage(Wrapper) {Message = message}.Write();*/
		}

		/// <summary>
		/// Send a chat message.
		/// </summary>
		/// <param name="message"></param>
		public void SendChat(string message)
		{
			SendChat(new MessageComponent(message));
		}
		
		/// <summary>
		/// Send a chat message with a color.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="color"></param>
		public void SendChat(string message, ChatColor color)
		{
			SendChat("§" + color.Value + message);
		}

		/// <summary>
		/// Kick the player from Trestle with a reason.
		/// </summary>
		/// <param name="reason"></param>
		public void Kick(MessageComponent reason)
		{
		//	new Disconnect(Wrapper) {Reason = reason}.Write();
			SavePlayer();
		}

		/// <summary>
		/// Kick the player from Trestle.
		/// </summary>
		public void Kick()
		{
			Kick(new MessageComponent("Unknown reason."));
		}

		/// <summary>
		/// Tries to toggle the Operator status on the player and returns if it succeeded or not.
		/// </summary>
		/// <returns></returns>
		public bool ToggleOperatorStatus()
		{
			//string savename = ServerSettings.OnlineMode ? UUID : Username;
			//IsOperator = OperatorLoader.Toggle(savename.ToLower());
			return IsOperator;
		}

		/// <summary>
		/// Save the player's info.
		/// </summary>
		public void SavePlayer()
		{
			/*// TODO: Health
			//byte[] health = HealthManager.Export();
			byte[] inv = Inventory.GetBytes();
			DataBuffer buffer = new (new byte[0]);
			buffer.WriteDouble(Location.X);
			buffer.WriteDouble(Location.Y);
			buffer.WriteDouble(Location.Z);
			buffer.WriteFloat(Location.Yaw);
			buffer.WriteFloat(Location.Pitch);
			buffer.WriteBool(Location.OnGround);
			buffer.WriteVarInt((int)GameMode);
			//buffer.WriteVarInt(health.Length);
			//foreach (byte b in health)
			//{
				//buffer.WriteByte(b);
			//}
			buffer.WriteVarInt(inv.Length);
			foreach (byte b in inv)
			{
				buffer.WriteByte(b);
			}
			byte[] data = buffer.ExportWriter;
			data = Globals.Compress(data);
			string savename = ServerSettings.OnlineMode ? UUID : Username;
			File.WriteAllBytes("Players/" + savename + ".pdata", data);*/
		}

		/// <summary>
		/// Load the player's saved info.
		/// </summary>
		public void LoadPlayer()
		{
			string savename = Config.OnlineMode ? UUID : Username;
			if (File.Exists("Players/" + savename + ".pdata"))
			{
				byte[] data = File.ReadAllBytes("Players/" + savename + ".pdata");
				data = Globals.Decompress(data);
				MinecraftStream reader = new MinecraftStream(data);
				double x = reader.ReadDouble();
				double y = reader.ReadDouble();
				double z = reader.ReadDouble();
				float yaw = reader.ReadFloat();
				float pitch = reader.ReadFloat();
				bool onGround = reader.ReadBool();
				Location = new Location(x, y, z) {Yaw = yaw, Pitch = pitch, OnGround = onGround};
				//GameMode = (GameMode) reader.ReadVarInt();
				int healthLength = reader.ReadVarInt();
				byte[] healthData = reader.Read(healthLength);
				int inventoryLength = reader.ReadVarInt();
				byte[] inventoryData = reader.Read(inventoryLength);
				//HealthManager.Import(healthData);
				Inventory.Import(inventoryData);
			}
			else
			{
				Location = World.SpawnPoint;
			}
			IsLoaded = true;
		}
    }
}
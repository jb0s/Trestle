using System;
using System.Linq;
using Trestle.Utils;
using Trestle.Enums;
using Trestle.World;
using Trestle.Networking;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32.SafeHandles;
using Trestle.Block;
using Trestle.Inventory.Inventories;
using Trestle.Networking.Packets.Play.Client;

namespace Trestle.Entity
{
    public class Player : Entity
    {
        /// <summary>
        /// The chunks this entity has loaded.
        /// </summary>
        public Dictionary<Tuple<int, int>, byte[]> ChunksUsed;
        
        /// <summary>
        /// The position of the current chunk the player is in.
        /// </summary>
        private Vector2 _currentChunkPosition = new(0, 0);
        
        /// <summary>
        /// The player's inventory.
        /// </summary>
        public PlayerInventory Inventory;
        
        /// <summary>
        /// The player's skin blob.
        /// </summary>
        public string SkinBlob = "";

        /// <summary>
		/// The player's username
		/// </summary>
		public string Username { get; set; }
		
		/// <summary>
		/// The player's Uuid
		/// </summary>
		public string Uuid { get; set; }
		
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
		public bool IsDigging { get; set; }

		/// <summary>
		/// Is the player crouching?
		/// </summary>
		public bool IsCrouching
		{
			get => (Metadata.Status & EntityStatus.Crouched) != 0;
			set
			{
				if (value)
					Metadata.Status |= EntityStatus.Crouched;
				else
					Metadata.Status &= ~EntityStatus.Crouched;
				
				World.BroadcastPacket(new EntityMetadata(Client.Player), Client.Player);
			}
		}
		
		/// <summary>
		/// Is the player sprinting?
		/// </summary>
		public bool IsSprinting
		{
			get => (Metadata.Status & EntityStatus.Sprinting) != 0;
			set
			{
				if (value)
					Metadata.Status |= EntityStatus.Sprinting;
				else
					Metadata.Status &= ~EntityStatus.Sprinting;
				
				World.BroadcastPacket(new EntityMetadata(Client.Player), Client.Player);
			}
		}

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
		public SkinParts SkinParts { get; set; }
		
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
		public EntityActionType LastEntityActionType { get; set; }

		/// <summary>
		/// Creates a player instance and registers it to the assigned world.
		/// You need to call the <see cref="InitializePlayer"/> function to spawn it as an entity.
		/// </summary>
		/// <param name="world"></param>
        public Player(World.World world) : base(EntityType.Player, world)
		{
			ChunksUsed = new Dictionary<Tuple<int, int>, byte[]>();
            Inventory = new PlayerInventory(this);
            World = world;

            Metadata = new PlayerMetadata(this);
		}
        
        /// <summary>
        /// Initializes the Player entity.
        /// </summary>
        internal void InitializePlayer()
        {
	        if (!Load())
		        Location = World.Spawnpoint;
	        
	        Client.SendPacket(new JoinGame(this));

	        TrestleServer.RegisterPlayer(Client);
			
			Client.SendPacket(new SpawnPosition(this));
	        Client.SendPacket(new PlayerPositionAndLook(Location));
	        
	        HasSpawned = true;
			
	        Inventory.SendToPlayer();
        }

        #region Updates

        /// <summary>
        /// Sends the player to a world.
        /// </summary>
        /// <param name="world"></param>
        public void SendToWorld(World.World world)
        {
			// Remove the player from the old world.
			World.RemoveEntity(this);

			// Update the world, and add the player to it.
	        World = world;
	        World.AddPlayer(this);
	        
	        SpawnForPlayers(World.Players.Values.ToArray());
	        foreach (var player in World.Players.Values)
	        {
		        player.SpawnForPlayers(new Player[1] { this });
	        }
        }

        public override void SpawnForPlayers(Player[] players)
	        => World.BroadcastPacket(new SpawnPlayer(this), this);

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
            
			if (yaw != 0.0f && pitch != 0.0f)
	        {
		        Location.Yaw = yaw;
		        Location.Pitch = pitch;
		        Location.HeadYaw = (byte)(yaw * 256 / 360);
	        }

			var prevLocation = Location;
	        Location.X = location.X;
	        Location.Y = location.Y;
	        Location.Z = location.Z;
	        Location.OnGround = onGround;
	        
	        _currentChunkPosition.X = (int) location.X >> 4;
	        _currentChunkPosition.Z = (int) location.Z >> 4;
	        
	        if (originalchunkcoords != _currentChunkPosition)
		        SendChunksForLocation(_currentChunkPosition);

	        if(prevLocation.DistanceTo(Location) < 8)
				World.BroadcastPacket(new EntityLookAndRelativeMove(EntityId, prevLocation, Location), this);
			else
				World.BroadcastPacket(new EntityTeleport(EntityId, Location));
	        
	        LookChanged();
        }
        
        /// <summary>
        /// Handler for when the client changes look direction.
        /// </summary>
        public void LookChanged()
	        => World.BroadcastPacket(new EntityHeadLook(EntityId, Location.HeadYaw), this);
        
        #endregion

        #region Animations

        /// <summary>
        /// Makes the player swing their hand.
        /// </summary>
        /// <param name="hand"></param>
        public void PlayerHandSwing(int hand)
			=> PlayerAnimation(AnimationType.SwingArm, hand);

		/// <summary>
		/// Performs an animationType on the client.
		/// </summary>
		/// <param name="animationType"></param>
		/// <param name="hand"></param>
		public void PlayerAnimation(AnimationType animationType, int hand = 0)
		{
			Client.Player.World.BroadcastPacket(new Animation(Client.Player, AnimationType.SwingArm), Client.Player);
		}

        #endregion

        #region Inventory

        #endregion

        #region Game modes

        /// <summary>
        /// Sets the player's game mode.
        /// </summary>
        /// <param name="target"></param>
        public void SetGamemode(GameMode target)
			=> SetGamemode(target, false);
		
		/// <summary>
		/// Silently sets the player's game mode.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="silent"></param>
		public void SetGamemode(GameMode target, bool silent)
		{
			GameMode = target;

            Client.SendPacket(new ChangeGameState(GameStateReason.ChangeGameMode, (int)target));
			
			if (!silent)
			{
				Logger.Info($"Updated {Username}'s gamemode to '{target}'");
				SendChat($"{ChatColor.Gray}Your gamemode was updated to {ChatColor.Aqua}{target}{ChatColor.Gray}!");
			}
		}

		/// <summary>
		/// Respawns the player entity.
		/// </summary>
		public void Respawn()
		{
			// TODO: [Survival] Add this
		}

		#endregion

		#region Chunk loading

		public void SendChunksForLocation(bool force = false)
			=> SendChunksForLocation(new Vector2((int)Location.X >> 4, (int)Location.Z >> 4), force);
		
		/// <summary>
		/// Send cached chunk for position.
		/// </summary>
		/// <param name="force"></param>
		public void SendChunksForLocation(Vector2 location, bool force = false)
		{
			if (!force && !HasSpawned)
				return;

			Client.ThreadPool.LaunchThread(() =>
			{
				foreach (var chunk in World.GenerateChunks(new Vector2(location.X, location.Z), ChunksUsed, ViewDistance))
				{
					if (Client != null && Client.TcpClient.Connected)
					{
						Client.SendPacket(new ChunkData(chunk));
						GetEntitysInChunk(_currentChunkPosition.X, _currentChunkPosition.Z);
					}
				}
			});
		}

		/// <summary>
		/// Gets the closest chunk to the player.
		/// </summary>
		public ChunkColumn GetChunk()
		{
			int chunkX = (int)Location.X >> 4;
			int chunkZ = (int)Location.Z >> 4;
			ChunkColumn chunk = World.WorldGenerator.GenerateChunkColumn(new Vector2(chunkX, chunkZ));
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
					Client.SendPacket(new SpawnPlayer(player));
				}
			}

			foreach (var entity in World.Entities.Values.ToArray())
			{
				var x = (int)entity.Location.X >> 4;
				var z = (int)entity.Location.Z >> 4;
				if (chunkX == x && chunkZ == z)
				{
					// TODO: [MP] Add this (Spawn Entity)
				}
			}

			// TODO: [WorldGen] Tile entities
		}
		
		#endregion

        #region Chat

        /// <summary>
        /// Send a chat message.
        /// </summary>
        /// <param name="message"></param>
        public void SendChat(MessageComponent message)
			=> Client.SendPacket(new ChatMessage(message));

		/// <summary>
		/// Send a chat message.
		/// </summary>
		/// <param name="message"></param>
		public void SendChat(string message)
			=> SendChat(new MessageComponent(message));
		
		/// <summary>
		/// Send a chat message with a color.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="color"></param>
		public void SendChat(string message, ChatColor color)
			=> SendChat(color.Value + message);

        #endregion

        #region Saving and loading

        public void Save()
        {
	        byte[] saveData;
	        using (var stream = new MinecraftStream())
	        {
		        // Chunk position
		        stream.WriteInt(_currentChunkPosition.X);
		        stream.WriteInt(_currentChunkPosition.Z);
		        
		        // Location
		        stream.WriteLocation(Location);
		        
		        // Inventory
		        stream.Write(Inventory.Export());

		        saveData = stream.Data;
	        }
	        
	        // Write the data to a file.
	        File.WriteAllBytes($"Players/{Uuid}", saveData);
        }

        public bool Load()
        {
	        string filePath = $"Players/{Uuid}";

	        if (!File.Exists(filePath))
		        return false;
	        
	        byte[] saveData = File.ReadAllBytes($"Players/{Uuid}");
	        using (var stream = new MinecraftStream(saveData))
	        {
		        _currentChunkPosition = new Vector2(stream.ReadInt(), stream.ReadInt());

		        Location = stream.ReadLocation();
		        Inventory.Import(stream.Read(6 * Inventory.Slots.Length));
	        }

	        return true;
        }

        #endregion

        #region Kicking

        /// <summary>
        /// Kick the player from Trestle with a reason.
        /// </summary>
        /// <param name="reason"></param>
        public void Kick(MessageComponent reason)
			=> Client.SendPacket(new Disconnect(reason));

		/// <summary>
		/// Kick the player from Trestle.
		/// </summary>
		public void Kick()
			=> Kick(new MessageComponent("Kicked by an operator."));
		
		/// <summary>
		/// Kick the player from Trestle for cheating suspicions.
		/// </summary>
		public void KickAntiCheat(string reason)
			=> Kick(new MessageComponent($"{ChatColor.Aqua}Trestle AC {ChatColor.Gray}- {ChatColor.Red}you've been kicked for cheating!\n\n{ChatColor.Reset}{reason}"));

        #endregion
    }
}
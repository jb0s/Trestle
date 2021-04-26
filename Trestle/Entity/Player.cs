using System;
using System.IO;
using System.Linq;
using Trestle.Block;
using Trestle.Utils;
using Trestle.Enums;
using Trestle.Items;
using Trestle.Worlds;
using Trestle.Networking;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using Trestle.Inventory.Inventories;
using Trestle.Networking.Packets.Play.Client;
using EntityStatus = Trestle.Networking.Packets.Play.Client.EntityStatus;

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
			get => (Metadata.Status & Enums.EntityStatus.Crouched) != 0;
			set
			{
				if (value)
					Metadata.Status |= Enums.EntityStatus.Crouched;
				else
					Metadata.Status &= ~Enums.EntityStatus.Crouched;
				
				World.BroadcastPacket(new EntityMetadata(Client.Player), Client.Player);
			}
		}
		
		/// <summary>
		/// Is the player sprinting?
		/// </summary>
		public bool IsSprinting
		{
			get => (Metadata.Status & Enums.EntityStatus.Sprinting) != 0;
			set
			{
				if (value)
					Metadata.Status |= Enums.EntityStatus.Sprinting;
				else
					Metadata.Status &= ~Enums.EntityStatus.Sprinting;
				
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
		/// The maximum amount of health the player can have.
		/// Should always equal to 20.
		/// </summary>
		public override int MaxHealth => 20;

		/// <summary>
		/// Creates a player instance and registers it to the assigned world.
		/// You need to call the <see cref="InitializePlayer"/> function to spawn it as an entity.
		/// </summary>
		/// <param name="world"></param>
        public Player(Worlds.World world) : base(EntityType.Player, world)
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
	        // If there's no save data to be loaded / failed to load, default to the spawnpoint.
	        if (!Load())
		        Location = World.Spawnpoint;
	        
	        // Tell the client it can join the game.
	        Client.SendPacket(new JoinGame(this));

	        // Register the new player in the player database.
	        TrestleServer.RegisterPlayer(Client);
			
	        // Spawn the player.
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
        public void SendToWorld(Worlds.World world)
        {
			// Remove the player from the old world.
			World.RemoveEntity(this);

			// Update the world, and add the player to it.
	        World = world;
	        World.AddPlayer(this);
	        
	        SpawnForPlayers(World.Players.Values.ToArray());
	        foreach (var player in World.Players.Values)
		        player.SpawnForPlayers(new Player[1] { this });

	        Inventory.Broadcast();
        }

        public override void SpawnForPlayers(Player[] players)
        {
	        World.BroadcastPacket(new SpawnPlayer(this), this);
	        Inventory.Broadcast();
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

        /// <summary>
        /// Teleports the player to another location.
        /// </summary>
        /// <param name="location"></param>
        public void Teleport(Vector3 location)
        {
	        PositionChanged(location, Location.Yaw, Location.Pitch, Location.OnGround);
	        Client.SendPacket(new PlayerPositionAndLook(Location));
	        ForceChunkReload = true;
        }
        
        #endregion

        #region Animations
        
        /// <summary>
        /// Makes the player swing their hand.
        /// </summary>
        /// <param name="hand"></param>
        public void PlayerHandSwing(int hand)
			=> PlayerAnimation(AnimationType.SwingArm, hand, this);

		/// <summary>
		/// Performs an animationType on the client.
		/// </summary>
		/// <param name="animationType"></param>
		/// <param name="hand"></param>
		public void PlayerAnimation(AnimationType animationType, int hand = 0, Player except = null)
			=> World.BroadcastPacket(new Animation(this, animationType), except);

        #endregion

        #region Inventory

        /// <summary>
        /// Attempts to use a totem.
        /// </summary>
        public bool AttemptTotem()
        {
	        Logger.Debug($"{Username}: Attempting to use a totem");
	        
	        if (Inventory.CurrentItem == new ItemStack(Material.TotemOfUndying, 1) || Inventory.Slots[45] == new ItemStack(Material.TotemOfUndying, 1))
	        {
		        Logger.Debug($"{Username}: Used a totem!");

		        // Reset health.
		        HealthManager.Reset();
		        
		        // Totem animation
		        Client.SendPacket(new EntityStatus(EntityId, 35));
		        World.BroadcastPacket(new Particle(47, (float)Location.X, (float)Location.Y, (float)Location.Z));
		        Inventory.RemoveItem((short)Material.TotemOfUndying, 1, 0);
		        return true;
	        }
	        
	        Logger.Debug($"{Username}: No totems in my inventory.");
	        return false;
        }
        
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
			HealthManager.Reset();
			Client.SendPacket(new Respawn());
			Teleport(World.Spawnpoint.ToVector3());
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
					//Client.SendPacket(new SpawnPlayer(player));
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
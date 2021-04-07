﻿using System;
using System.Linq;
using Trestle.Utils;
using Trestle.Enums;
using Trestle.World;
using Trestle.Networking;
using System.Collections.Generic;
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
		public bool Digging { get; set; }

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
		public EntityActionType LastEntityActionType { get; set; }

		/// <summary>
		/// Creates a player instance and registers it to the assigned world.
		/// You need to call the <see cref="InitializePlayer"/> function to spawn it as an entity.
		/// </summary>
		/// <param name="world"></param>
        public Player(World.World world) : base(-1, world)
		{
			ChunksUsed = new Dictionary<Tuple<int, int>, byte[]>();
            Inventory = new InventoryManager(this);
            World = world;

            Metadata = new PlayerMetadata();
		}
        
        /// <summary>
        /// Initializes the Player entity.
        /// </summary>
        internal void InitializePlayer()
        {
	        Client.SendPacket(new JoinGame(this));

	        SendToWorld(World);
			
			Globals.RegisterPlayer(Client);
	        
	        Client.SendPacket(new SpawnPosition(this));
	        Client.SendPacket(new PlayerPositionAndLook(Location));
	        
	        HasSpawned = true;
			
	        Inventory.SendToPlayer();
	        BroadcastInventory();

	        //SetGamemode(World.DefaultGamemode, true);
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
	        
			// Teleport the player to the world spawnpoint.
	        Location = world.Spawnpoint;
	        
	        SpawnForPlayers(World.Players.Values.ToArray());
	        foreach (var player in World.Players.Values)
	        {
		        player.SpawnForPlayers(new Player[1] { this });
	        }
        }

        public override void SpawnForPlayers(Player[] players)
        {
	        World.BroadcastPacket(new SpawnPlayer(this), this);
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
	        
	        _currentChunkPosition.X = (int) location.X / 16;
	        _currentChunkPosition.Z = (int) location.Z / 16;
	        
	        if (originalchunkcoords != _currentChunkPosition)
		        SendChunksForLocation(_currentChunkPosition);
	        
	        if(prevLocation.DistanceTo(Location) < 8)
				World.BroadcastPacket(new EntityLookAndRelativeMove(EntityId, prevLocation, Location), this);

	        LookChanged();
        }
        
        /// <summary>
        /// Handler for when the client changes look direction.
        /// </summary>
        public void LookChanged()
        {
	        World.BroadcastPacket(new EntityHeadLook(EntityId, Location.HeadYaw), this);
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

        #endregion

        #region Animations

        /// <summary>
        /// Makes the player swing their hand.
        /// </summary>
        /// <param name="hand"></param>
        public void PlayerHandSwing(byte hand)
			=> PlayerAnimation(AnimationType.SwingArm, hand);

		/// <summary>
		/// Performs an animationType on the client.
		/// </summary>
		/// <param name="animationType"></param>
		/// <param name="hand"></param>
		public void PlayerAnimation(AnimationType animationType, byte hand = 0)
		{
			// TODO: [MP] Add this
		}

        #endregion

        #region Inventory

        /// <summary>
        /// Sends the player's inventory to the client.
        /// </summary>
        public void BroadcastInventory()
		{
			// TODO: [MP] Add this
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
				Console.WriteLine(Username + "'s gamemode was changed to " + target.ToString("D"));
				SendChat("Your gamemode was changed to " + target.ToString());
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
        {
			SendChunksForLocation(new Vector2((int)Location.X / 16, (int)Location.Z / 16), force);
		}
		
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
					// TODO: [MP] Add this (Spawn Player)
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
			=> Kick(new MessageComponent($"{ChatColor.Gray}[{ChatColor.Aqua}TresGuard{ChatColor.Gray}] {ChatColor.Reset}{reason}"));

        #endregion
    }
}
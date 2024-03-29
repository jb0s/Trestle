using System;
using System.Linq;
using Trestle.Enums;
using Trestle.Utils;
using Trestle.Entity;
using System.Threading;
using Trestle.Networking;
using System.Diagnostics;
using Trestle.Worlds;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using Trestle.Block.Blocks;
using Trestle.Networking.Packets.Play.Client;

namespace Trestle.Worlds
{
    public class World : IDisposable
    {
	    /// <summary>
	    /// List of Players in this world.
	    /// </summary>
        public ConcurrentDictionary<int, Player> Players { get; } 
	    
	    /// <summary>
	    /// List of Entities in this world.
	    /// </summary>
        public ConcurrentDictionary<int, Entity.Entity> Entities { get; } 
	    
	    /// <summary>
	    /// List of Windows in this world.
	    /// </summary>
	    public ConcurrentDictionary<int, Inventory.Inventory> Windows { get; } 

	    /// <summary>
	    /// The type of this world.
	    /// </summary>
        public WorldType Type { get; }
	    
	    /// <summary>
	    /// The location in which players will (re)spawn in.
	    /// </summary>
        public Location Spawnpoint { get; set; }
	    
	    /// <summary>
	    /// The default gamemode for this world.
	    /// </summary>
        public GameMode DefaultGamemode { get; set; } = GameMode.Creative;
        
	    /// <summary>
	    /// The world's generator.
	    /// </summary>
	    public WorldGenerator WorldGenerator { get; }
	    
	    /// <summary>
	    /// The amount of players in this world.
	    /// </summary>
        public int PlayerCount => Players.Count;

	    /// <summary>
	    /// Amount of times the world has been ticked.
	    /// </summary>
        public long Ticks = 0;
	    
	    /// <summary>
	    /// Timer used for ticking the world.
	    /// </summary>
        private Timer TickTimer { get; set; }
	    
	    /// <summary>
	    /// Dummy object used to check if another script is ticking the world.
	    /// </summary>
        private object _tickLock = new();
	    
	    /// <summary>
	    /// Is the world disposed of or not?
	    /// </summary>
        private bool _disposed = false;

	    public World(WorldType type, WorldGenerator worldGenerator)
        {
            Type = type;
            WorldGenerator = worldGenerator;
            Players = new ConcurrentDictionary<int, Player>();
            Entities = new ConcurrentDictionary<int, Entity.Entity>();
            Windows = new ConcurrentDictionary<int, Inventory.Inventory>();

            Spawnpoint = worldGenerator.GetSpawnPoint();
        }

	    #region Utilities

	    /// <summary>
	    /// Pre-loads spawn chunks for new players.
	    /// </summary>
	    public void Initialize()
	    {
		    // Start a counter for seeing how long it takes to generate spawn chunks.
		    var chunkLoading = Stopwatch.StartNew();

		    // Load the spawn chunks with View Distance 8.
		    GenerateChunks(Vector2.ToChunkLocation(Spawnpoint), new Dictionary<Tuple<int, int>, byte[]>(), 8);
            
		    // Stop the timer and log.
		    chunkLoading.Stop();
		    Logger.Debug($"Loaded spawn chunks in {chunkLoading.ElapsedMilliseconds}ms");

		    // Start ticking the world.
		    TickTimer = new Timer(OnTick, null, 50, 50);
	    }
	    
	    /// <summary>
	    /// Attempts to tick the world.
	    /// </summary>
	    /// <param type="state"></param>
	    private void OnTick(object state)
	    {
		    // If we can't get a lock on the tickLock dummy object, don't tick the world.
		    // This usually means something else is already ticking this world, and we don't need it twice.
		    if (!Monitor.TryEnter(_tickLock))
			    return;

		    try
		    {
			    // Tick every player.
			    foreach (var player in Players.Values.ToArray())
				    player.OnTick();
			    
			    // Tick every entity.
			    foreach (var entity in Entities.Values.ToArray())
				    entity.OnTick();
		    }
		    finally
		    {
			    // Release the lock on the dummy object and increment the tick count.
			    Monitor.Exit(_tickLock);
			    Ticks++;
		    }
	    }

	    #endregion

        #region Entities

        /// <summary>
        /// Add an entity to the world.
        /// </summary>
        /// <param type="entity"></param>
        public void AddEntity(Entity.Entity entity)
        {
	        Entities.TryAdd(entity.EntityId, entity);
        }

        /// <summary>
        /// Remove an entity from the world.
        /// </summary>
        /// <param type="entity"></param>
        public void RemoveEntity(Entity.Entity entity)
        {
	        Entities.TryRemove(entity.EntityId, out _);
        }
        
        /// <summary>
        /// Add a player to the world.
        /// </summary>
        /// <param type="player">The new player.</param>
        public virtual void AddPlayer(Player newPlayer)
        {
	        if (Players.TryAdd(newPlayer.EntityId, newPlayer))
	        {
		        BroadcastPacket(new SpawnPlayer(newPlayer), newPlayer);

		        foreach (var player in Players.Values)
			        newPlayer.Client.SendPacket(new SpawnPlayer(player));
		        
		        // Mark the player as spawned and broadcast the inventory.
		        newPlayer.Inventory.Broadcast();
		        newPlayer.IsSpawned = true;
	        }
        }
        
        /// <summary>
        /// Remove a player from the world.
        /// </summary>
        /// <param type="player">The new player.</param>
        public virtual void RemovePlayer(Player player)
        {
	        if (Players.TryRemove(player.EntityId, out _))
		        BroadcastPacket(new DestroyEntities(new int[1] { player.EntityId }));

	        player.IsSpawned = false;
        }

        #endregion

        #region Chunk Generation

        /// <summary>
        /// Generates chunks at `chunkPosition` in a radius of `radius`
        /// </summary>
        /// <param type="chunkPosition">The position of the central chunk.</param>
        /// <param type="chunksUsed">A list of chunks that are already loaded.</param>
        /// <param type="radius">The radius of chunks to generate around the central chunk.</param>
        public IEnumerable<byte[]> GenerateChunks(Vector2 chunkPosition, Dictionary<Tuple<int, int>, byte[]> chunksUsed, double radius)
        {
	        lock (chunksUsed)
	        {
		        Dictionary<Tuple<int, int>, double> newOrders = new Dictionary<Tuple<int, int>, double>();

		        double radiusSquared = Math.Pow(radius, 2);

		        int centerX = chunkPosition.X;
		        int centerZ = chunkPosition.Z;

		        for (double x = -radius; x <= radius; ++x)
		        {
			        for (double z = -radius; z <= radius; ++z)
			        {
				        var distance = (x*x) + (z*z);
				        int chunkX = (int) (x + centerX);
				        int chunkZ = (int) (z + centerZ);
				        Tuple<int, int> index = new Tuple<int, int>(chunkX, chunkZ);
				        newOrders[index] = distance;
			        }
		        }

		        foreach (var chunkKey in chunksUsed.Keys.ToArray())
		        {
			        if (!newOrders.ContainsKey(chunkKey))
				        chunksUsed.Remove(chunkKey);
		        }

		        foreach (var pair in newOrders.OrderBy(pair => pair.Value))
		        {
			        if (chunksUsed.ContainsKey(pair.Key)) continue;

			        if (WorldGenerator == null) continue;

			        byte[] chunk = null;
			        if (File.Exists($"worlds/{Type.ToString().ToLower()}/{new Vector2(pair.Key.Item1, pair.Key.Item2).ToString()}"))
			        {
				        ChunkColumn chunkColumn = WorldGenerator.LoadChunkFromSave(new Vector2(pair.Key.Item1, pair.Key.Item2), $"worlds/{Type.ToString().ToLower()}/{new Vector2(pair.Key.Item1, pair.Key.Item2).ToString()}");
				        chunk = chunkColumn?.Export();
			        }
			        else
			        {
				        ChunkColumn chunkColumn = WorldGenerator.GenerateChunkColumn(new Vector2(pair.Key.Item1, pair.Key.Item2));
				        chunk = chunkColumn?.Export();
			        }

			        chunksUsed.Add(pair.Key, chunk);
			        yield return chunk;
		        }
	        }
        }

        #endregion

        #region Blocks
        public Block.Block GetBlock(Vector3 location, bool cacheChunk = false)
        {
	        ChunkColumn chunk;
	        
	        if(cacheChunk)
		        chunk = WorldGenerator.CreateAndCache(new Vector2((int)location.X >> 4, (int)location.Z >> 4));
	        else
		        chunk = WorldGenerator.GenerateChunkColumn(new Vector2((int)location.X >> 4, (int)location.Z >> 4));
	        
	        var material = chunk.GetBlock(new Vector3(Mod(location.X), (int) location.Y, Mod(location.Z)));
	        var data = chunk.GetBlockData(new Vector3(Mod(location.X), (int) location.Y, Mod(location.Z)));

	        if (material == Material.Chest)
		        return new Chest()
		        {
			        Metadata = data,
			        Coordinates = location
		        };
	        
	        return new Block.Block(material)
	        {
		        Metadata = data,
		        Coordinates = location
	        };;
        }
        
        public void SetBlock(Vector3 location, Material block)
        {
	        var chunk = WorldGenerator.GenerateChunkColumn(new Vector2((int)location.X >> 4, (int)location.Z >> 4));
	        
	        chunk.SetBlock(new Vector3(Mod(location.X), (int) location.Y, Mod(location.Z)), block);
	        var data = chunk.GetBlockData(new Vector3(Mod(location.X), (int) location.Y, Mod(location.Z)));
		        
	        var material = chunk.GetBlock(new Vector3(Mod(location.X), (int)location.Y, Mod(location.Z)));
		        
	        BroadcastPacket(new BlockChange(new Vector3(location.X, (int) location.Y, location.Z), block, data));
        }
			
        private int Mod(double val)
	        => (int)(((val%16) + 16)%16);
        
        #endregion

        #region Packets

        /// <summary>
        /// Broadcasts a packet to every player in this world.
        /// </summary>
        public void BroadcastPacket(Packet packet)
        {
	        foreach (var player in Players)
	        {
		        player.Value.Client.SendPacket(packet);
	        }
        }
        
        /// <summary>
        /// Broadcasts a packet to every player in this world except the player specified as `except.`
        /// </summary>
        public void BroadcastPacket(Packet packet, Player except)
        {
	        foreach (var player in Players)
	        {
		        if(player.Value != except)
					player.Value.Client.SendPacket(packet);
	        }
        }
        
        
        /// <summary>
        /// Broadcasts a chat message to every player in this world.
        /// </summary>
        /// <param type="message">The message to broadcast.</param>
        /// <param type="chatType">The location for the message to appear in.</param>
        /// <param type="sender">The sender of the message.</param>
        public void BroadcastChat(MessageComponent message, ChatMessageType chatType)
			=> BroadcastPacket(new ChatMessage(message, chatType));

        #endregion

        #region Utilities

        private int ChunkRound(int number)
        {
	        // Smaller multiple
	        int a = (number / 16) * 16;
     
	        // Larger multiple
	        int b = a + 16;
 
	        // Return of closest of two
	        return (number - a > b - number)? b : a;
        }

        #endregion

        #region Saving & loading

        public void Save()
        {
	        // IF the world doesn't have a save directory yet, create one.
	        if (!Directory.Exists($"worlds/{Type.ToString().ToLower()}"))
		        Directory.CreateDirectory($"worlds/{Type.ToString().ToLower()}");

	        foreach (var chunk in WorldGenerator.Chunks.Values)
	        {
		        if (chunk.IsDirty)
		        {
			        File.WriteAllBytes($"worlds/{Type.ToString().ToLower()}/{chunk.Coordinates.ToString()}", chunk.Export());
			        chunk.IsDirty = false;
		        }
	        }
        }

        #endregion
        
        private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_disposed) return;
				_disposed = true;

				TickTimer.Dispose();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~World()
			=> Dispose(false);
    }
}
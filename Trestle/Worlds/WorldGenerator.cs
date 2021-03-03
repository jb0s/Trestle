using System;
using System.Linq;
using Trestle.Utils;
using Trestle.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using Trestle.Networking.Packets;

namespace Trestle.Worlds
{
    public class WorldGenerator
    {
        public World MyWorld { get; set; }
        public virtual bool IsCaching { get; set; }

        public WorldGenerator(World myWorld)
        {
            MyWorld = myWorld;
        }
        
        public virtual void Initialize()
            => throw new NotImplementedException();

        public virtual Chunk GenerateChunk(Vector2 location)
            => throw new NotImplementedException();

        public virtual Vector3 GetSpawnPoint()
            => throw new NotImplementedException();
        
        public IEnumerable<Chunk> GenerateChunks(int viewDistance, List<Tuple<int, int>> chunksUsed, Player player)
        {
            lock (chunksUsed)
            {
                Dictionary<Tuple<int, int>, double> newOrders = new Dictionary<Tuple<int, int>, double>();
                double radiusSquared = viewDistance / Math.PI;
                double radius = Math.Ceiling(Math.Sqrt(radiusSquared));
                int centerX = (int)player.Location.X >> 4;
                int centerZ = (int)player.Location.Z >> 4;

                for (double x = -radius; x <= radius; ++x)
                {
                    for (double z = -radius; z <= radius; ++z)
                    {
                        var distance = (x * x) + (z * z);
                        if (distance > radiusSquared)
                        {
                            continue;
                        }
                        int chunkX = (int)(x + centerX);
                        int chunkZ = (int)(z + centerZ);
                        Tuple<int, int> index = new Tuple<int, int>(chunkX, chunkZ);
                        newOrders[index] = distance;
                    }
                }

                if (newOrders.Count > viewDistance)
                {
                    foreach (var pair in newOrders.OrderByDescending(pair => pair.Value))
                    {
                        if (newOrders.Count <= viewDistance) break;
                        newOrders.Remove(pair.Key);
                    }
                }

                foreach (var chunkKey in chunksUsed.ToArray())
                {
                    if (!newOrders.ContainsKey(chunkKey))
                    {
                        chunksUsed.Remove(chunkKey);
                        new Task(() => player.UnloadChunk(chunkKey.Item1, chunkKey.Item2)).Start();
                    }
                }

                foreach (var pair in newOrders.OrderBy(pair => pair.Value))
                {
                    if (chunksUsed.Contains(pair.Key)) continue;

                    var chunk = GenerateChunk(new Vector2(pair.Key.Item1, pair.Key.Item2));
                    chunksUsed.Add(pair.Key);

                    yield return chunk;
                }
            }
        }
        
        public virtual IEnumerable<Chunk> GenerateChunks(int viewDistance, double playerX, double playerZ,
            List<Tuple<int, int>> chunksUsed, Player player, bool output = false)
        {
            lock (chunksUsed)
            {
                var newOrders = new Dictionary<Tuple<int, int>, double>();
                var radiusSquared = viewDistance / Math.PI;
                var radius = Math.Ceiling(Math.Sqrt(radiusSquared));
                var centerX = (int)(playerX) >> 4;
                var centerZ = (int)(playerZ) >> 4;

                for (var x = -radius; x <= radius; ++x)
                {
                    for (var z = -radius; z <= radius; ++z)
                    {
                        var distance = (x * x) + (z * z);
                        if (distance > radiusSquared)
                        {
                            continue;
                        }
                        var chunkX = (int)(x + centerX);
                        var chunkZ = (int)(z + centerZ);
                        var index = new Tuple<int, int>(chunkX, chunkZ);
                        newOrders[index] = distance;
                    }
                }

                if (newOrders.Count > viewDistance)
                {
                    foreach (var pair in newOrders.OrderByDescending(pair => pair.Value))
                    {
                        if (newOrders.Count <= viewDistance) break;
                        newOrders.Remove(pair.Key);
                    }
                }

                foreach (var chunkKey in chunksUsed.ToArray())
                {
                    if (!newOrders.ContainsKey(chunkKey))
                    {
                        List<ForcedBlock> forcedStructuresForChunk = new List<ForcedBlock>();
                        if (MyWorld.BlocksToForce.ContainsKey($"{chunkKey.Item1}{chunkKey.Item2}"))
                            forcedStructuresForChunk = MyWorld.BlocksToForce[$"{chunkKey.Item1}{chunkKey.Item2}"];
                        
                        /*
                        new ChunkData(player.Wrapper)
                        {
                            Queue = false,
                            Unloader = true,
                            Chunk = new Chunk(MyWorld, chunkKey.Item1, chunkKey.Item2, forcedStructuresForChunk)
                        }.Write();*/
                        
                      //  new UpdateViewPosition(player.Wrapper).Write();

                        chunksUsed.Remove(chunkKey);
                    }
                }

                foreach (var pair in newOrders.OrderBy(pair => pair.Value))
                {
                    if (chunksUsed.Contains(pair.Key)) continue;

                    var chunk = GenerateChunk(new Vector2(pair.Key.Item1, pair.Key.Item2));
                    chunksUsed.Add(pair.Key); 

                    yield return chunk;
                }
            }
        }
        
        public virtual void SaveChunks(string folder)
        {
            throw new NotImplementedException();
        }

        public virtual Chunk LoadChunk(int x, int y)
        {
            throw new NotImplementedException();
        }
    }
}
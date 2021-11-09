using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Trestle.Levels.Attributes;
using Trestle.Levels.Biomes;
using Trestle.Levels.Dimensions;
using Trestle.Levels.Enums;
using Trestle.Levels.Generators;

namespace Trestle.Levels.Services
{
    public interface ILevelService
    {
        public void Ping();
        Level CreateLevel(string name, DimensionType type = DimensionType.Overworld, LevelType levelType = LevelType.Flat);
        Level GetDefaultLevel();
    }
    
    public class LevelService : ILevelService
    {
        public ConcurrentDictionary<string, Level> Levels { get; } = new();
        
        private readonly Dictionary<DimensionType, Dimension> _dimensions = new();
        private readonly Dictionary<BiomeType, Biome> _biomes = new();

        private readonly ILogger<LevelService> _logger;
        
        public LevelService(ILogger<LevelService> logger)
        {
            _logger = logger;

            RegisterDimensionsAndBiomes();
            PrepareLevels();
        }

        public void Ping()
        {
        }

        private void PrepareLevels()
        {
            _logger.LogInformation("Preparing levels...");
            CreateLevel("world");
        }
        
        public Level CreateLevel(string name, DimensionType dimensionType = DimensionType.Overworld, LevelType levelType = LevelType.Flat)
        {
            if (Levels.ContainsKey(name))
                throw new ArgumentException("Level name already in use.", nameof(name));

            Generator generator = (dimensionType, levelType) switch
            {
                (DimensionType.Overworld, LevelType.Flat) => new FlatGenerator(),
                _ => throw new ArgumentOutOfRangeException()
            };

            Levels[name] = new Level(_dimensions[dimensionType], name, generator);
            return Levels[name];
        }

        public Level GetDefaultLevel()
            => Levels.FirstOrDefault().Value;
        
        private void RegisterDimensionsAndBiomes()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var dimensionAttribute = type.GetCustomAttribute<DimensionAttribute>();
                var biomeAttribute = type.GetCustomAttribute<BiomeAttribute>();

                if (dimensionAttribute != null)
                    _dimensions[dimensionAttribute.DimensionType] = (Dimension)Activator.CreateInstance(type);
                else if (biomeAttribute != null)
                    _biomes[biomeAttribute.BiomeType] = (Biome)Activator.CreateInstance(type);
            }
        }
    }
}
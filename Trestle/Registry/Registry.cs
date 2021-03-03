using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace Trestle.Registry
{
    public class Registry
    {
        public List<Entry<Dimension>> DimensionRegistry { get; set; }
        
        public List<Entry<Biome>> BiomeRegistry { get; set; }

        public Registry()
        {
            DimensionRegistry = new ();
            BiomeRegistry = new ();
            
            var dimensions = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Trestle.Assets.dimensions.json")).ReadToEnd();
            var biomes = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Trestle.Assets.biomes.json")).ReadToEnd();

            DimensionRegistry = JsonSerializer.Deserialize<List<Entry<Dimension>>>(dimensions);
            BiomeRegistry = JsonSerializer.Deserialize<List<Entry<Biome>>>(biomes);
        }
    }
}
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Trestle.Configuration.Models;

namespace Trestle.Configuration.Service
{
    public interface IConfigService
    {
        public Task<ServerConfig> GetConfig();
        
        public Task Save();
        public Task Load();
    }
    
    public class ConfigService : IConfigService
    {
        private ILogger<ConfigService> _logService;
        private ServerConfig _configuration;
        
        public ConfigService(ILogger<ConfigService> logService)
        {
            _logService = logService;
        }

        public async Task<ServerConfig> GetConfig()
        {
            if (_configuration == null)
                await Load();

            return _configuration;
        }
        
        /// <summary>
        /// Saves the config to a file.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task Save()
        {
            if (_configuration == null)
                throw new InvalidOperationException("Can't save server configuration if it's not loaded first.");
            
            // Open the config file.
            using(var file = File.OpenWrite("config.json"))
            {
                // Serialize the json to the config file.
                await JsonSerializer.SerializeAsync(file, _configuration, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                
                file.Close();
            }
        }
        
        /// <summary>
        /// Loads the server configuration from a file, or creates a new one if the file isn't present.
        /// </summary>
        public async Task Load()
        {
            // If config file doesn't exist, create one.
            if (!File.Exists("config.json"))
            {
                _logService.LogInformation("No configuration file found, creating one now.");

                _configuration = new ServerConfig();
                await Save();
            }

            // Open the config file to a stream.
            var content = File.OpenRead("config.json");
            
            // Attempt to deserialize the config file to a class instance.
            try 
            {
                _configuration = await JsonSerializer.DeserializeAsync<ServerConfig>(content);
            }
            catch (JsonException exception) // Deserialization failed, close the file stream, delete the file and create a new one.
            {
                _logService.LogWarning("Config file is corrupt! Creating a new one.");
                
                // Close the file and delete it.
                content?.Close();
                File.Delete("config.json");
                
                // Restart the function, which in turn makes a new config.
                _ = Load();
            }
        }
    }
}
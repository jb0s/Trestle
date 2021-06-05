using System;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Trestle.Configuration.Models;

namespace Trestle.Configuration.Service
{
    public interface IConfigService
    {
        public ServerConfig GetConfig();
        
        public void Save();
        public void Load();
    }
    
    public class ConfigService : IConfigService
    {
        private ILogger<ConfigService> _logService;
        private ServerConfig _configuration;
        
        public ConfigService(ILogger<ConfigService> logService)
        {
            _logService = logService;
        }

        /// <summary>
        /// Returns the configuration.
        /// This is put in a function so that you can get the config through the <see cref="IConfigService"/> interface.
        /// </summary>
        /// <returns></returns>
        public ServerConfig GetConfig()
        {
            if(_configuration == null)
                Load();

            return _configuration;
        }
        
        /// <summary>
        /// Saves the config to a file.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void Save()
        {
            // If the config isn't loaded yet, load it first.
            if (_configuration == null)
                Load();
            
            string serialized = JsonSerializer.Serialize(_configuration, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            
            File.WriteAllText("config.json", serialized);
        }
        
        /// <summary>
        /// Loads the server configuration from a file, or creates a new one if the file isn't present.
        /// </summary>
        public void Load()
        {
            // If config file doesn't exist, create one.
            if (!File.Exists("config.json"))
            {
                _logService.LogInformation("No configuration file found, creating one now.");

                _configuration = new ServerConfig();
                Save();
            }

            // Open the config file to a stream.
            string content = File.ReadAllText("config.json");
            
            // Attempt to deserialize the config file to a class instance.
            try 
            {
                _configuration = JsonSerializer.Deserialize<ServerConfig>(content);
            }
            catch (JsonException exception) // Deserialization failed, close the file stream, delete the file and create a new one.
            {
                _logService.LogWarning("Config file is corrupt! Creating a new one.");
                
                // Close the file and delete it.
                File.Delete("config.json");
                
                // Restart the function, which in turn makes a new config.
                Load();
            }
        }
    }
}
using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Trestle.Utils;

namespace Trestle.Networking.Services
{
    public interface IMojangService
    {
        Uuid GetUuid(string username);
    }
    
    public class MojangService : IMojangService
    {
        public Uuid GetUuid(string username)
        {
            try
            {
                var client = new WebClient();
                var profile =
                    JsonSerializer.Deserialize<Profile>(
                        client.DownloadString(Endpoints.Mojang.GetUuid(username)));
                return Guid.Parse(profile.Id);
            }
            catch (WebException)
            {
                return Guid.Empty;
            }
        }
    }

    public class Profile
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
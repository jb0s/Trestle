using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trestle.Networking;

namespace Trestle.Utils
{
    public class Mojang
    {
        public static UserProfile HasJoined(Client client)
        {
            try
            {
                var uri = new Uri(
                    string.Format(
                        "https://sessionserver.mojang.com/session/minecraft/hasJoined?username={0}&serverId={1}",
                        client.Username,
                        PacketCryptography.JavaHexDigest(Encoding.UTF8.GetBytes("")
                            .Concat(client.SharedKey)
                            .Concat(PacketCryptography.PublicKeyToAsn1(Globals.ServerKey))
                            .ToArray())
                    ));

                var data = new WebClient().DownloadString(uri);
                
                // Checks if the response has no content, indicating that they haven't joined.
                if (string.IsNullOrEmpty(data))
                    return null;

                var profile = JsonSerializer.Deserialize<UserProfile>(data);
                return profile;
            }
            catch
            {
                return null;
            }
        }

        public static UserProfile GetProfileByUsername(string username)
        {
            try
            {
                var uri = new Uri(
                    string.Format(
                        "https://api.mojang.com/users/profiles/minecraft/{0}",
                        username
                    ));

                var data = new WebClient().DownloadString(uri);
                
                // Checks if the response has no content, indicating that they haven't joined.
                if (string.IsNullOrEmpty(data))
                    return null;

                var profile = JsonSerializer.Deserialize<UserProfile>(data);
                return profile;
            }
            catch
            {
                return null;
            }
        }
        
        public static UserProfile GetProfileById(string id)
        {
            try
            {
                var uri = new Uri(
                    string.Format(
                        "https://sessionserver.mojang.com/session/minecraft/profile/{0}?unsigned=false",
                        id
                    ));

                var data = new WebClient().DownloadString(uri);
                
                // Checks if the response has no content, indicating that they haven't joined.
                if (string.IsNullOrEmpty(data))
                    return null;

                var profile = JsonSerializer.Deserialize<UserProfile>(data);
                return profile;
            }
            catch
            {
                return null;
            }
        }
    }
    
    public class UserProfile
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("properties")]
        public UserProfileProperty[] Properties { get; set; }
    }
    
    public class UserProfileProperty
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("value")]
        public string Value { get; set; }
        
        [JsonPropertyName("signature")]
        public string Signature { get; set; }
    }

}
using System.Text.Json.Serialization;

namespace Trestle.Utils
{
    public class MessageComponent
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = "";
        
        [JsonPropertyName("bold")]
        public bool Bold = false;
        
        [JsonPropertyName("italic")]
        public bool Italic = false;
        
        [JsonPropertyName("underlined")]
        public bool Underlined = false;
        
        [JsonPropertyName("strikethrough")]
        public bool Strikethrough = false;
        
        [JsonPropertyName("obfuscated")]
        public bool Obfuscated = false;

        public MessageComponent(string message)
        {
            Text = message;
        }
    }
}
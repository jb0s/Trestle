using System.Text.Json.Serialization;

namespace Trestle.Utils
{
    public class MessageComponent
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = "";
        public bool bold = false;
        public bool italic = false;
        public bool underlined = false;
        public bool strikethrough = false;
        public bool obfuscated = false;

        public MessageComponent(string message)
        {
            Text = message;
        }
    }
}
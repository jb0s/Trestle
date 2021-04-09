using System;
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
            Text = Text.Replace("&nl", "\n").Replace("&", "§");
        }

        public string RemoveColors()
        {
            var split = Text.Split('§');
            
            if (split.Length == 1)
                return Text;
            
            var final = "";
            foreach (var str in Text.Split('§'))
            {
                if (string.IsNullOrEmpty(str))
                    continue;
                
                final += str.Remove(0,1);
            }
            
            return final;
        }
    }
}
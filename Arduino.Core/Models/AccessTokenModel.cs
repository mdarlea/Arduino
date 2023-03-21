using Newtonsoft.Json;

namespace Arduino.Core.Models
{
    internal class AccessTokenModel
    {
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string? TokenType { get; set; }
    }
}

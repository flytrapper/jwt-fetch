using System.Text.Json.Serialization;

namespace JwtFetch
{
    public class JwtResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
    }
}

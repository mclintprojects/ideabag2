using Newtonsoft.Json;

namespace ProgrammingIdeas.Models
{
    internal class LoginResponseData
    {
        [JsonProperty("idToken")]
        public int Token { get; set; }

        public string Email { get; set; }

        [JsonProperty("expiresIn")]
        public long ExpiresInMillis { get; set; }

        [JsonProperty("localId")]
        public string UserId { get; set; }
    }
}
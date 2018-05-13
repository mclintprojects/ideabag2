using Newtonsoft.Json;

namespace ProgrammingIdeas.Models
{
    internal class LoginData
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("returnSecureToken")]
        public bool ReturnSecureToken { get; set; } = true;
    }
}
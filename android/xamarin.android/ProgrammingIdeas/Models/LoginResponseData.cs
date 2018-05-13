using Newtonsoft.Json;

namespace ProgrammingIdeas.Models
{
    public class LoginResponseData
    {
        [JsonProperty("idToken")]
        public string Token { get; set; }

        public string Email { get; set; }

        [JsonProperty("expiresIn")]
        public string ExpiresIn { get; set; }

        public double ExpiresInMillis => double.Parse(ExpiresIn);

        [JsonProperty("localId")]
        public string UserId { get; set; }
    }
}
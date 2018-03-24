using Newtonsoft.Json;

namespace ProgrammingIdeas.Models
{
    internal class IdeaComment
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        public string Author { get; set; }
        public string Comment { get; set; }
        public long Created { get; set; }
    }
}
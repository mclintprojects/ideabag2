using Newtonsoft.Json;

namespace ProgrammingIdeas.Models
{
    internal class IdeaComment
    {
        [JsonProperty("userId")]
        public string Id { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("created")]
        public long Created { get; set; }
    }
}
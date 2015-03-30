using Newtonsoft.Json;

namespace ScrapeFinra.Models
{
    class APIData
    {
        [JsonProperty("html")]
        public string html { get; set; }
    }
}

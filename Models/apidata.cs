using Newtonsoft.Json;

namespace ScrapeFinra.Models
{
    class APIData
    {
        [JsonProperty("html")]
        string html { get; set; }
    }
}

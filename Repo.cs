using Newtonsoft.Json;

namespace StudentContributionConsole
{
    public class Repo
    {
        [JsonProperty("name")] 
        public string Name { get; set; }

        [JsonProperty("html_url")] 
        public string HtmlUrl { get; set; }
    }
}
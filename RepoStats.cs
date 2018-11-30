using Newtonsoft.Json;

namespace StudentContributionConsole
{
    public partial class RepoStats
    {
        public static RepoStats[] FromJson(string json) => JsonConvert.DeserializeObject<RepoStats[]>(json, StudentContributionConsole.Converter.Settings);
    }

    public partial class RepoStats
    {
        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("weeks")]
        public Week[] Weeks { get; set; }

        [JsonProperty("author")]
        public Author Author { get; set; }
    }
}
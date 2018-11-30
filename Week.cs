using Newtonsoft.Json;

namespace StudentContributionConsole
{
    public class Week
    {
        [JsonProperty("w")]
        public long W { get; set; }

        [JsonProperty("a")]
        public long A { get; set; }

        [JsonProperty("d")]
        public long D { get; set; }

        [JsonProperty("c")]
        public long C { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.ComTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Octokit;
using RestSharp;
using RestSharp.Extensions;

namespace StudentContributionConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var requestContext = new RequestContext();
            requestContext.GetCommitsForRepo("repos/long-island-iced-tea/bangazon/stats/contributors");
        }
    }

    public class RequestContext
    {
        private string baseUrl = "https://api.github.com/";
        
        public void GetCommitsForRepo(string requestEndpoint)
        {
            var client = new RestClient(baseUrl);
            
            var request = new RestRequest();
            request.Resource = requestEndpoint;
            request.AddHeader("user-agent", "NSS-Commit-Tracker (Darwin 18.0.0 Darwin Kernel Version 18.0.0: Wed Aug 22 20:13:40 PDT 2018; root:xnu-4903.201.2~1/RELEASE_X86_64; x64; en-US;)");

            var response = client.Execute(request);

            var deserializedResult = Welcome.FromJson(response.Content);

            Console.ReadKey();
        }

        private List<ContributorStats> ConvertToObject(JObject obj)
        {
            var contributors = new List<ContributorStats>();
            foreach (var item in obj)
            {
                var contributor = new ContributorStats();
                contributor.Total = obj[0].Value<int>("total");
                //contributor.Weeks = obj[0].Values<CommitStats>("weeks").ToList();
                contributor.Author = obj[0]["author"].Value<string>("login");
                
                contributors.Add(contributor);
            }

            return contributors;
        }
    }
    
        public partial class Welcome
    {
        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("weeks")]
        public Week[] Weeks { get; set; }

        [JsonProperty("author")]
        public Author Author { get; set; }
    }

    public partial class Author
    {
        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("node_id")]
        public string NodeId { get; set; }

        [JsonProperty("avatar_url")]
        public Uri AvatarUrl { get; set; }

        [JsonProperty("gravatar_id")]
        public string GravatarId { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("html_url")]
        public Uri HtmlUrl { get; set; }

        [JsonProperty("followers_url")]
        public Uri FollowersUrl { get; set; }

        [JsonProperty("following_url")]
        public string FollowingUrl { get; set; }

        [JsonProperty("gists_url")]
        public string GistsUrl { get; set; }

        [JsonProperty("starred_url")]
        public string StarredUrl { get; set; }

        [JsonProperty("subscriptions_url")]
        public Uri SubscriptionsUrl { get; set; }

        [JsonProperty("organizations_url")]
        public Uri OrganizationsUrl { get; set; }

        [JsonProperty("repos_url")]
        public Uri ReposUrl { get; set; }

        [JsonProperty("events_url")]
        public string EventsUrl { get; set; }

        [JsonProperty("received_events_url")]
        public Uri ReceivedEventsUrl { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("site_admin")]
        public bool SiteAdmin { get; set; }
    }

    public partial class Week
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

    public partial class Welcome
    {
        public static Welcome[] FromJson(string json) => JsonConvert.DeserializeObject<Welcome[]>(json, StudentContributionConsole.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Welcome[] self) => JsonConvert.SerializeObject(self, StudentContributionConsole.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}

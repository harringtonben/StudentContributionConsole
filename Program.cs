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
using Newtonsoft.Json.Serialization;
using Octokit;
using RestSharp;
using RestSharp.Extensions;

namespace StudentContributionConsole
{
    class Program
    {
        private static readonly Dictionary<string, string> groupNames = new Dictionary<string, string>
        {
            {"Long Island Iced Tea", "long-island-iced-tea"},
            {"Fuzzy Navel", "fuzzy-navel"},
            {"Salty Chiwawa", "SaltyChiwawa"},
            {"Three Legged Monkey", "three-legged-monkey"}
        };

        static void Main(string[] args)
        {
            var run = true;

            while (run)
            {
                Console.WriteLine("Which group would you like to see commit history for?");
    
                var counter = 0;
                
                foreach (var group in groupNames)
                {
                    counter++;
                    Console.WriteLine($"{counter}. {group.Key}");
                }
    
                Console.Write("Please select an option from above and hit enter: ");
                var selectedGroup = Console.ReadLine();
                var selectedGroupKey = int.Parse(selectedGroup) - 1;
    
    
                var requestContext = new RequestContext();

                if (groupNames.Values.ElementAtOrDefault(selectedGroupKey) == null)
                {
                    Console.Clear();
                    continue;
                }

                var reposToCheck = requestContext.GetReposForGroup(groupNames.Values.ElementAt(selectedGroupKey));
    
                counter = 0;
                
                foreach (var repo in reposToCheck)
                {
                    counter++;
                    Console.WriteLine($"{counter}. {repo.Name}: {repo.HtmlUrl}");
                }
                
                Console.Write("Please select an option from above and hit enter: ");
                var selectedRepo = Console.ReadLine();
                var selectedRepoKey = int.Parse(selectedRepo) - 1;
                

    
                var commitStats = requestContext.GetCommitsForRepo($"repos/{groupNames.Values.ElementAt(selectedGroupKey)}/{reposToCheck[selectedRepoKey].Name}/stats/contributors");

                if (commitStats.Length == 0)
                {
                    Console.WriteLine("Sorry, either there is no contribution information for this repo, or the Github API is being trash. Press enter and let's try this again!");
                    Console.ReadLine();
                    Console.Clear();
                    continue;
                }

                
                
                foreach (var result in commitStats)
                {
                    var totalAdditions = result.Weeks.Sum(x => x.A);
                    var totalDeletions = result.Weeks.Sum(x => x.D);
                    Console.WriteLine($"{result.Author.Login} has made {result.Total} commits since {DateTimeOffset.FromUnixTimeSeconds(result.Weeks[0].W)} with {totalAdditions} added lines of code and {totalDeletions} deleted lines of code");
                }
    
                Console.Write("Type q and then enter to quit or enter to check another repo: ");
                var continueApp = Console.ReadLine();
    
                if (continueApp == "q")
                    run = false;
                
                Console.Clear();
                
            }
        }
    }

    public class RequestContext
    {
        private string baseUrl = "https://api.github.com/";
        
        public RepoStats[] GetCommitsForRepo(string requestEndpoint)
        {
            var client = new RestClient(baseUrl);
            
            var request = new RestRequest();
            request.Resource = requestEndpoint;
            request.AddHeader("user-agent", "NSS-Commit-Tracker (Darwin 18.0.0 Darwin Kernel Version 18.0.0: Wed Aug 22 20:13:40 PDT 2018; root:xnu-4903.201.2~1/RELEASE_X86_64; x64; en-US;)");

            IRestResponse response;
            try
            {
                response = client.Execute(request);
            }
            finally
            {
                response = client.Execute(request);
            }
           
            if (response.StatusCode != HttpStatusCode.OK)
            {
                    Console.Clear();
                    return new RepoStats[0]; 
            }
            
            var deserializedResult = RepoStats.FromJson(response.Content);

            return deserializedResult;

        }

        public List<Repo> GetReposForGroup(string orgName)
        {
            var client = new RestClient(baseUrl);
            var request = new RestRequest();
            request.Resource = $"users/{orgName}/repos";
            request.AddHeader("user-agent", "NSS-Commit-Tracker (Darwin 18.0.0 Darwin Kernel Version 18.0.0: Wed Aug 22 20:13:40 PDT 2018; root:xnu-4903.201.2~1/RELEASE_X86_64; x64; en-US;)");

            var response = client.Execute(request);
           
            var deserializedResult = JsonConvert.DeserializeObject<List<Repo>>(response.Content);
            return deserializedResult; 
            
        }
    }

    public class Repo
    {
        [JsonProperty("name")] 
        public string Name { get; set; }

        [JsonProperty("html_url")] 
        public string HtmlUrl { get; set; }
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

    public class Author
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

    public partial class RepoStats
    {
        public static RepoStats[] FromJson(string json) => JsonConvert.DeserializeObject<RepoStats[]>(json, StudentContributionConsole.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this RepoStats[] self) => JsonConvert.SerializeObject(self, StudentContributionConsole.Converter.Settings);
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

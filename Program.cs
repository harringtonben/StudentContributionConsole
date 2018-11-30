using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.ComTypes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Octokit;
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
}

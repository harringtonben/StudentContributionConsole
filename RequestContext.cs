using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using RestSharp;

namespace StudentContributionConsole
{
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
}
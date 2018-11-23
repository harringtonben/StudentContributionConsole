using System;
using System.Collections.Generic;
using Octokit;

namespace StudentContributionConsole
{
    public class ContributorStats
    {
        public int Total { get; set; }
        public List<CommitStats> Weeks { get; set; }
        public string Author { get; set; }
    }

    public class CommitStats
    {
        public int W { get; set; }
        public int A { get; set; }
        public int D { get; set; }
        public int C { get; set; }
    }

}
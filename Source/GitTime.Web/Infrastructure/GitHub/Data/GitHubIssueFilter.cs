using System;

namespace GitTime.Web.Infrastructure.GitHub.Data
{
    [Serializable]
    public class GitHubIssueFilter
    {
        public Int32? AssigneeID { get; set; }
        public String RepositoryName { get; set; }
        public String[] AvailableRepositories { get; set; }
        public GitHubIssueStateType? State { get; set; }
    }
}
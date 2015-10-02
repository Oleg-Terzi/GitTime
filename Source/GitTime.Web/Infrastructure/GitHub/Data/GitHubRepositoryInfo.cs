using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitTime.Web.Infrastructure.GitHub.Data
{
    public class GitHubRepositoryInfo
    {
        #region Simple properties

        public Int32 ID { get; set; }
        public String Name { get; set; }
        public String FullName { get; set; }
        public Boolean IsPrivate { get; set; }
        public String Url { get; set; }
        public String Description { get; set; }
        public Boolean IsFork { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? PushedOn { get; set; }
        public String HomepageUrl { get; set; }
        public Int32 Size { get; set; }
        public Int32 StargazersCount { get; set; }
        public Int32 WatchersCount { get; set; }
        public String Language { get; set; }
        public Boolean HasIssues { get; set; }
        public Boolean HasDownloads { get; set; }
        public Boolean HasWiki { get; set; }
        public Boolean HasPages { get; set; }
        public Int32 ForksCount { get; set; }
        public String MirrorUrl { get; set; }
        public Int32 OpenIssuesCount { get; set; }
        public String DefaultBranchName { get; set; }

        #endregion

        #region Entities

        public GitHubUserInfo Owner { get; set; }

        #endregion
    }
}
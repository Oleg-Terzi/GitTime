using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitTime.Web.Infrastructure.GitHub.Data
{
    [Serializable]
    public class GitHubIssueFilter
    {
        public Int32? AssigneeID { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitTime.Web.Infrastructure.GitHub.Data
{
    public class GitHubIssueInfo
    {
        #region Simple properties

        public Int32 ID { get; set; }
        public String IssueUrl { get; set; }
        public Int32 Number { get; set; }
        public String Title { get; set; }
        public GitHubIssueStateType State { get; set; }
        public Boolean IsLocked { get; set; }
        public Int32 CommentsCount { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? ClosedOn { get; set; }
        public String BodyText { get; set; }

        #endregion

        #region Entities

        public GitHubUserInfo User { get; set; }
        public GitHubLabelInfo[] Labels { get; set; }
        public GitHubUserInfo Assignee { get; set; }
        public GitHubRepositoryInfo Repository { get; set; }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitTime.Web.Infrastructure.GitHub.Data
{
    public class GitHubCommentInfo
    {
        public Int32 ID { get; set; }
        public String Url { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public String BodyText { get; set; }

        public GitHubUserInfo Author { get; set; }

    }
}
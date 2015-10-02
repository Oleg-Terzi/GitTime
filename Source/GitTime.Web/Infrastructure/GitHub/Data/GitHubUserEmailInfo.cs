using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitTime.Web.Infrastructure.GitHub.Data
{
    public class GitHubUserEmailInfo
    {
        #region Simple properties

        public String Email { get; set; }
        public Boolean IsPrimary { get; set; }
        public Boolean IsVerified { get; set; }

        #endregion
    }
}
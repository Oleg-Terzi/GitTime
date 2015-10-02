using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitTime.Web.Infrastructure.GitHub.Data
{
    public class GitHubUserInfo
    {
        #region Simple properties

        public Int32 ID { get; set; }
        public String LoginName { get; set; }
        public String AvatarUrl { get; set; }
        public String GravatarId { get; set; }
        public String ProfileUrl { get; set; }
        public GitHubUserType AccountType { get; set; }
        public Boolean IsSiteAdmin { get; set; }
        public String Name { get; set; }
        public String Company { get; set; }
        public String BlogUrl { get; set; }
        public String Location { get; set; }
        public String Email { get; set; }
        public Boolean? IsHireable { get; set; }
        public String Biography { get; set; }
        public Int32 PublicReposCount { get; set; }
        public Int32 PublicGistsCount { get; set; }
        public Int32 FollowersCount { get; set; }
        public Int32 FollowingCount { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        #endregion

        #region Overriden methods

        public override String ToString()
        {
            return LoginName ?? base.ToString();
        }

        #endregion
    }
}
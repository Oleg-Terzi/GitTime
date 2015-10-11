using System;

namespace GitTime.Web.Models.View.Issue
{
    public class SearchCriteriaModel
    {
        public String RepositoryName { get; set; }
        public String StatusName { get; set; }

        public Boolean Clear { get; set; }
    }
}
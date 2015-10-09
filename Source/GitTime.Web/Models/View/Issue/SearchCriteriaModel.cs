using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitTime.Web.Models.View.Issue
{
    public class SearchCriteriaModel
    {
        public Int32? AssigneeID { get; set; }

        public Boolean Clear { get; set; }
    }
}
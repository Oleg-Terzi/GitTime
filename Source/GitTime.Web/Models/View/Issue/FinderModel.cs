using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitTime.Web.Models.View.Issue
{
    public class FinderModel
    {
        public Int32? Key { get; set; }

        public virtual BaseSearchResultsModel SearchResults { get; set; }
        public virtual SearchCriteriaModel SearchCriteria { get; set; }
        public virtual EditorModel Edit { get; set; }
    }
}
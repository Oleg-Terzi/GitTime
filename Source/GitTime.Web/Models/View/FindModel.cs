using System;
using System.ComponentModel.DataAnnotations;

namespace GitTime.Web.Models.View
{
    public class FindModel
    {
        #region Classes

        public class SearchResultsModel
        {
            public string Filter { get; set; }
            public int? PageIndex { get; set; }
        }

        public class SearchCriteriaModel
        {
            public string Action { get; set; }

            public int? ProjectID { get; set; }

            public int? PersonContactID { get; set; }

            public DateTime? EntryDateFrom { get; set; }

            public DateTime? EntryDateThru { get; set; }

            public bool Clear { get; set; }
        }

        public class EditModel
        {
            public int? ID { get; set; }

            public DateTime EntryDate { get; set; }

            [Required]
            public int? PersonContactID { get; set; }

            [Required]
            public int? ProjectID { get; set; }

            public int? IssueNumber { get; set; }

            public string IssueDescription { get; set; }

            [Required]
            public decimal? Hours { get; set; }
        }

        #endregion

        #region Properties

        public int? Key { get; set; }

        public virtual SearchResultsModel SearchResults { get; set; }
        public virtual SearchCriteriaModel SearchCriteria { get; set; }
        public virtual EditModel Edit { get; set; }

        #endregion
    }
}
using System;

namespace GitTime.Web.Models.View.Timecard
{
    public class SearchCriteriaModel
    {
        public string Action { get; set; }

        public int? ProjectID { get; set; }

        public int? PersonContactID { get; set; }

        public DateTime? EntryDateFrom { get; set; }

        public DateTime? EntryDateThru { get; set; }

        public bool Clear { get; set; }
    }
}
using System;

namespace GitTime.Web.Models.View.Timecard
{
    public class SearchCriteriaModel
    {
        public String Action { get; set; }

        public Int32? ProjectID { get; set; }

        public Int32? PersonContactID { get; set; }

        public DateTime? EntryDateFrom { get; set; }

        public DateTime? EntryDateThru { get; set; }

        public Boolean Clear { get; set; }
    }
}
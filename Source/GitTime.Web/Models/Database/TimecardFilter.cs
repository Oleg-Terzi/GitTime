using System;

namespace GitTime.Web.Models.Database
{
    [Serializable]
    public class TimecardFilter
    {
        public int? ProjectID { get; set; }
        public int? PersonContactID { get; set; }
        public DateTime? EntryDateFrom { get; set; }
        public DateTime? EntryDateThru { get; set; }
    }
}
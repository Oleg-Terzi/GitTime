using System;

namespace GitTime.Web.Models.Database
{
    [Serializable]
    public class TimecardFilter
    {
        public Int32? ProjectID { get; set; }
        public Int32? PersonContactID { get; set; }
        public DateTime? EntryDateFrom { get; set; }
        public DateTime? EntryDateThru { get; set; }
    }
}
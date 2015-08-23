using System;

namespace GitTime.Web.Models.Database
{
    [Serializable]
    public class TimecardFilter
    {
        public string ProjectName { get; set; }
        public string PersonName { get; set; }
        public DateTime? EntryDateFrom { get; set; }
        public DateTime? EntryDateThru { get; set; }
    }
}
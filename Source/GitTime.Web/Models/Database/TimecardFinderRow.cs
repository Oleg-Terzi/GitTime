using System;

namespace GitTime.Web.Models.Database
{
    public class TimecardFinderRow
    {
        public Int32 TimecardID { get; set; }
        public DateTime EntryDate { get; set; }
        public String PersonName { get; set; }
        public String Repository { get; set; }
        public String CompanyName { get; set; }
        public String IssueDescription { get; set; }
        public Decimal Hours { get; set; }
    }
}
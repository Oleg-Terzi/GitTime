using System;

namespace GitTime.Web.Models.Database
{
    public class TimecardFinderRow
    {
        public int TimecardID { get; set; }
        public DateTime EntryDate { get; set; }
        public string PersonName { get; set;}
        public string Repository { get; set; }
        public string CompanyName { get; set; }
        public string IssueDescription { get; set; }
        public decimal Hours { get; set; }
    }
}
using System;

namespace GitTime.Web.Models.Database
{
    public class ProjectFinderRow
    {
        public Int32 ProjectID { get; set; }
        public String CompanyName { get; set; }
        public String ProjectName { get; set; }
        public String Repository { get; set; }
    }
}
using System;

namespace GitTime.Web.Models.Database
{
    public class ContactFinderRow
    {
        public Int32 ContactID { get; set; }
        public String Name { get; set; }
        public String Email { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Subtype { get; set; }
    }
}
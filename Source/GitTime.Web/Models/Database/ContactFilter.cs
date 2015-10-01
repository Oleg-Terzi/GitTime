using System;

namespace GitTime.Web.Models.Database
{
    [Serializable]
    public class ContactFilter
    {
        public String Subtype { get; set; }
        public String Name { get; set; }
        public String PersonName { get; set; }
    }
}
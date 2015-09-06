using System;

namespace GitTime.Web.Models.Database
{
    [Serializable]
    public class ContactFilter
    {
        public string Subtype { get; set; }
        public string Name { get; set; }
        public string PersonName { get; set; }
    }
}
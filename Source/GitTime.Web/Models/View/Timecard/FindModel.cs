using System;
using System.ComponentModel.DataAnnotations;

namespace GitTime.Web.Models.View.Timecard
{
    public class FindModel
    {
        public string Action { get; set; }

        public string ProjectName { get; set; }

        public string PersonName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateEntryFrom { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateEntryThru { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace GitTime.Web.Models.View.Project
{
    public class EditorModel
    {
        public Int32? ID { get; set; }

        [Required]
        public Int32? CompanyContactID { get; set; }

        [Required]
        [MaxLength(128)]
        public String Name { get; set; }

        public String Description { get; set; }

        [Required]
        [MaxLength(128)]
        public String Repository { get; set; }
    }
}
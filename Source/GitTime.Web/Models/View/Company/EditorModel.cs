using System;
using System.ComponentModel.DataAnnotations;

namespace GitTime.Web.Models.View.Company
{
    public class EditorModel
    {
        public Int32? ID { get; set; }

        [Required]
        [MaxLength(128)]
        public String Name { get; set; }
    }
}
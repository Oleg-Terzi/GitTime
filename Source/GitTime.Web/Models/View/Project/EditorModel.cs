using System.ComponentModel.DataAnnotations;

namespace GitTime.Web.Models.View.Project
{
    public class EditorModel
    {
        public int? ID { get; set; }

        [Required]
        public int? CompanyContactID { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [MaxLength(128)]
        public string Repository { get; set; }
    }
}
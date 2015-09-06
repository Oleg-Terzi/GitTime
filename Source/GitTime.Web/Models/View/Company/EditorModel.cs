using System.ComponentModel.DataAnnotations;

namespace GitTime.Web.Models.View.Company
{
    public class EditorModel
    {
        public int? ID { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }
    }
}
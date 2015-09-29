using System.ComponentModel.DataAnnotations;

namespace GitTime.Web.Models.Database
{
    public class Person : Contact
    {
        [Required]
        [MaxLength(128)]
        public string Email { get; set; }

        [Required]
        [MaxLength(64)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(64)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(128)]
        public string Password { get; set; }
    }
}
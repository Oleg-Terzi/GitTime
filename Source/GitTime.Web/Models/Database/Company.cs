using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GitTime.Web.Models.Database
{
    public class Company: Contact
    {
        #region Simple properties

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        #endregion

        #region Entities

        public virtual ICollection<Project> Projects { get; set; }

        #endregion

        #region Construction

        public Company()
        {
            Projects = new List<Project>();
        }

        #endregion
    }
}
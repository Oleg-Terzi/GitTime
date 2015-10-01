using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GitTime.Web.Models.Database
{
    public class Person : Contact
    {
        #region Simple properties

        [Required]
        [MaxLength(128)]
        public String Email { get; set; }

        [Required]
        [MaxLength(64)]
        public String FirstName { get; set; }

        [Required]
        [MaxLength(64)]
        public String LastName { get; set; }

        [Required]
        [MaxLength(128)]
        public String Password { get; set; }

        #endregion

        #region Entities

        public virtual ICollection<Role> Roles { get; set; }

        #endregion

        #region Construction

        public Person()
        {
            Roles = new List<Role>();
        }

        #endregion
    }
}
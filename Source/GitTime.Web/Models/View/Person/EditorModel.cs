using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GitTime.Web.Models.View.Person
{
    public class EditorModel
    {
        public Int32? ID { get; set; }

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

        public virtual ICollection<Int32> Roles { get; set; }

        #region Construction

        public EditorModel()
        {
            Roles = new List<Int32>();
        }

        #endregion
    }
}
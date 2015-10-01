using System;
using System.ComponentModel.DataAnnotations;

namespace GitTime.Web.Models.View
{
    public class LoginModel
    {
        [Required]
        public String Email { get; set; }

        [Required]
        public String Password { get; set; }

        public Boolean RememberMe { get; set; }
    }
}
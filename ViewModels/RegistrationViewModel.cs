using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Eshop.ViewModels
{
    public class RegistrationViewModel
    {   
        public User User { get; set; }

        public IEnumerable<User> Users { get; set; }

    

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail id is not valid")]
        public string Email { get; set; }

        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [Required(AllowEmptyStrings =false, ErrorMessage ="Password is required")]
        [DataType(DataType.Password)]

        public string Password { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]

        public string Username { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; } 
    }
}
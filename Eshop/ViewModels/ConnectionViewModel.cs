using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Eshop.ViewModels
{
    public class ConnectionViewModel
    {
        public User User { get; set; }

        public User Admin { get; set; }

        public Boolean IsActive { get; set; }

        public IEnumerable<User> Admins { get; set; }

        public IEnumerable<User> Users { get; set; }
        
        public Connection Connection { get; set; }

        public product Product { get; set; }
        public IEnumerable<product> Products { get; set; }
        public ILookup<int, favourite> Favourites { get; set; }


        //UNDER HERE ADDING THE SUPPORT VIEWMODELS PROPERTIES BECAUSE OF RESTRICTION WITH RENDER PARTIAL
        //    NEED TO FIX IN REFACTORING


        public int Id { get; set; }

        public int From { get; set; }

        public int To { get; set; }
        public string Message { get; set; }

        public DateTime DateSent { get; set; }

        public bool Read { get; set; }


        public virtual User FromUser { get; set; }

        public virtual User ToAdmin { get; set; }

        [Display(Name = "First Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter First Name")]
        public string FirstName { get; set; }


        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter Last Name")]
        public string LastName { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your email")]
        public string Email { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Inquiry can not be empty")]
        [StringLength(500, ErrorMessage = "Must be between 10 and 500 characters", MinimumLength = 10)]
        public string Inquiry { get; set; }
        public int FromId { get; set; }

        public IEnumerable<User> FromUsers { get; set; }
        public IEnumerable<User> FromAdmins { get; set; }
        //public string FromUserName { get; set; }

        //public string FromFirstName { get; set; }
        //public string FromLastName { get; set; }


        public string UserName { get; set; }

        public Nullable<int> UserId { get; set; }

        public IEnumerable<SupportMessage> SupportMessages { get; set; }
    }
}
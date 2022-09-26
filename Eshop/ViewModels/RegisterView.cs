using Eshop.Models.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Eshop.ViewModels
{
    public class RegisterView
    {
        public bool EmailExists { get; set; }

        public RegisterView()
        {

        }
        public RegisterView(UserDTO row)
        {
            UserId = row.UserId;
            FirstName = row.FirstName;
            LastName = row.LastName;
            Username = row.Username;
            Email = row.Email;
            Password = row.Password;
        }

        [Key]
        public int UserId { get; set; }

       // [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter FirstName")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

       // [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter LastName")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter User Name")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Display(Name ="Date of birth")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString ="{0:dd/MM/yyyy}")]
        public DateTime? DateOfBirth { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
      
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        public string Password { get; set; }

        [Display(Name = "Password Confirmation")]
        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please confirm the password")]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match")]
        public string Confirmpwd { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail id is not valid")]
        public string Email { get; set; }

        public bool IsEmailVerified { get; set; }
        [StringLength(14, ErrorMessage = "Must be between 10 and 14 characters", MinimumLength = 10)]
        public string Mobile { get; set; }

        [StringLength(14, ErrorMessage = "Must be between 10 and 14 characters", MinimumLength = 10)]
        public string Phone { get; set; }
        public Guid ActivationCode { get; set; }

    }
}
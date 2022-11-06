using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Eshop.ViewModels
{
    public class OrderViewModel
    {
        public System.DateTime CreatedAt { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter Shipping Method")]
        public int ShippingId { get; set; }


        public string _address { get; set; }




        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter Street")]
        public string Street { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter City")]
        public string City { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter Postal Code")]
        public string PostalCode { get; set; }

        public string TrackingNumber { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter First Name")]

        public string FirstName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter Last Name")]

        public string LastName { get; set; }

        public int PaymentType { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter Phone Number")]
        public string PhoneNumber { get; set; }

        public int TotalPrice { get; set; }

        public IEnumerable<cart> CartItems { get; set; }

        public int UserId { get; set; }
        public void Adress()
        {

            _address = $"{City} {Street} {PostalCode}";
        }
    }
}

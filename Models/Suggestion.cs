using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace Eshop.Models
{
    public class Suggestion
    {
        public Suggestion()
        {
            UserUsageOptions = new Dictionary<string, string>()
            {
                { "Simple use or future upgrade","Internet Browsing, Movies,Microsoft Office, Online course, Broswer games"},            
                { "Gaming built 1080p-1440p-UHD-4K","Compositions exclusivly for Gaming ,aimed at gpraphics performance "},
                {"Workstation","Compositions with focus on securing files and their processor performance"},
               
            };
        }
        [Display(Name = "What's your available budget?")]
        [Required(ErrorMessage = "Please let us know about your budget")]

        public double money { get; set; }
        public IDictionary<string, string> UserUsageOptions { get; set; }
        [Display(Name = "Hard disk total space")]
        public int HardDiscTotalSpace { get; set; }
        [Display(Name = "Choose the kind of your computer usage")]

        public string UserOption { get; set; }
    }
}
using Eshop.Models.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eshop.ViewModels
{   
   
    public class SupportMessageViewModel
    {   
        
       
        public int Id { get; set; }

        public int From { get; set; }

        public int To { get; set; }
        public string Message { get; set; }

        public DateTime DateSent { get; set; }

        public bool Read { get; set; }

        
        public virtual User FromUser { get; set; }
       
        public virtual User ToAdmin{ get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int FromId { get; set; }

        public IEnumerable<User> FromUsers { get; set; }
        public IEnumerable<User> FromAdmins { get; set; }
        //public string FromUserName { get; set; }

        //public string FromFirstName { get; set; }
        //public string FromLastName { get; set; }
        public int id { get; set; }

       
        public string UserName { get; set; }
      
        public Nullable<int> UserId { get; set; }

        public virtual User User { get; set; }
    }
}
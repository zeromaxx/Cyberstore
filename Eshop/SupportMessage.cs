//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Eshop
{
    using System;
    using System.Collections.Generic;
    
    public partial class SupportMessage
    {
        public SupportMessage()
        {

        }

        public SupportMessage(string message)
        {
            Message = message;
        }
        public int id { get; set; }
        public Nullable<int> From { get; set; }
        public Nullable<int> To { get; set; }
        public string Message { get; set; }
        public Nullable<System.DateTime> DateSent { get; set; }
        public Nullable<bool> Read { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public Nullable<int> FromId { get; set; }
        public Nullable<int> UserId { get; set; }
    
        public virtual User User { get; set; }
    }
}

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
    
    public partial class cpu
    {
        public int id { get; set; }
        public int productId { get; set; }
        public Nullable<int> cores { get; set; }
        public Nullable<int> threads { get; set; }
        public string model { get; set; }
        public string frequency { get; set; }
        public string chipset { get; set; }
        public string socket { get; set; }
    
        public virtual product product { get; set; }
    }
}
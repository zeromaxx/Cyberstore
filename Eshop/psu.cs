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
    
    public partial class psu
    {
        public int id { get; set; }
        public int productId { get; set; }
        public Nullable<int> watt { get; set; }
        public string connector { get; set; }
    
        public virtual product product { get; set; }
    }
}

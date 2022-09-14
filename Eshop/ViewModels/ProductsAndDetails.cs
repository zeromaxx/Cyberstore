using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eshop.ViewModels
{
    public class ProductsAndDetails
    {
        public IEnumerable<product> products { get; set; }
        public IEnumerable<cpu> cpus { get; set; }
        public string _Type { get; set; }
    }
}
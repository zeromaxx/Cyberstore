using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eshop.ViewModels
{
    public class ComparsionViewModel
    {
        public product SelectedProduct { get; set; }
        public product ProductToCompare { get; set; }
        
        public IEnumerable<product> SelectedList { get; set; }
        public cpu cpu1 { get; set; }
        public cpu cpu2 { get; set; }

        public motherboard Motherboard1 { get; set; }
        public motherboard Motherboard2 { get; set; }
        public gpu gpu1 { get; set; }
        public gpu gpu2 { get; set; }
        public psu psu1 { get; set; }
        public psu psu2 { get; set; }
        public ram ram1 { get; set; }
        public ram ram2 { get; set; }
        public hardDisc hardDisc1 { get; set; }
        public hardDisc hardDisc2 { get; set; }
        public monitor monitor1 { get; set; }
        public monitor monitor2 { get; set; }

        public bool TwoProducts { get; set; }

    }
}
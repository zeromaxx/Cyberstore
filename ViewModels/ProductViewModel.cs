using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eshop.ViewModels
{
    public class ProductViewModel
    {
        public IEnumerable<product> Products { get; set; }
        public IEnumerable<cpu> Cpus { get; set; }
        public IEnumerable<psu> Psus { get; set; }

        public IEnumerable<gpu> Gpus { get; set; }

        public IEnumerable<ram> Rams { get; set; }

        public IEnumerable<motherboard> Motherboards { get; set; }

        public IEnumerable<category> Categories { get; set; }

        public IEnumerable<hardDisc> HardDiscs { get; set; }
        public IEnumerable<monitor> monitors { get; set; }
        public IEnumerable<box> boxes { get; set; }


        public monitor Monitor { get; set; }
        public product Product { get; set; }
        public Desktop desktop { get; set; }
        public cpu Cpu { get; set; }
        public gpu Gpu { get; set; }
        public psu Psu { get; set; }
        public ram Ram { get; set; }
        public motherboard Motherboard { get; set; }
        public hardDisc HardDisc { get; set; }
        public category Category { get; set; }
        public box Box { get; set; }


        public HttpPostedFileBase Image1 { get; set; }
        public HttpPostedFileBase Image2 { get; set; }
        public HttpPostedFileBase Image3 { get; set; }
        public HttpPostedFileBase Thumbnail { get; set; }
        
        public int CategoryID { get; set; }
        public string _string { get; set; }
        public string Type { get; set; }
        public string Action { get; set; }
        public string img { get; set; }
        public int ProductID { get; set; }
        public int ItemID { get; set; }
    }
}
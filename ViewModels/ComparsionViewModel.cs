using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eshop.ViewModels
{
    public class ComparsionViewModel
    {
        public ComparsionViewModel()
        {
            MyList = new List<product>();
        }
       
        
        public List<product> MyList;
        public string type { get; set; }
        public string Partial { get; set; }

        public product product1 { get; set; }

        public product product2 { get; set; }

        public product product3 { get; set; }

        public product product4 { get; set; }

        public int id1 { get; set; }
       
        public int id2 { get; set; }
       
        public int id3 { get; set; }
       
        public int id4 { get; set; }


     




       
    



    }
}
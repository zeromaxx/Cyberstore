using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eshop.ViewModels
{
    public class DesktopViewModel  // JJJ New ViewModel
    {
        public DesktopViewModel()
        {
            ListOfDesktopProducts = new List<product>() { };
        }
        public string type = "None";
        public string Name { get; set; }
        public Desktop desktop { get; set; }
        public product monitor { get; set; }
       

        public IEnumerable<product> Products { get; set; }
        public IEnumerable<cpu> Cpus { get; set; }
        public IEnumerable<psu> Psus { get; set; }

        public IEnumerable<gpu> Gpus { get; set; }

        public IEnumerable<ram> Rams { get; set; }

        public IEnumerable<motherboard> Motherboards { get; set; }

        public IEnumerable<hardDisc> HardDiscs { get; set; }
        public IEnumerable<monitor> monitors { get; set; }
        public IEnumerable<box> boxes { get; set; }

        public IEnumerable<product> ProdMonitors { get; set; }
        public IEnumerable<product> ProdBoxes { get; set; }
        public IEnumerable<product> ProdCpus { get; set; }
        public IEnumerable<product> ProdPsus { get; set; }

        public IEnumerable<product> ProdGpus { get; set; }

        public IEnumerable<product> ProdRams { get; set; }

        public IEnumerable<product> ProdMotherboards { get; set; }

        public IEnumerable<product> ProdHardDiscs { get; set; }

        public int productID { get; set; }
        public List<product> ListOfDesktopProducts  { get; set; }
        public product NullProduct { get; set; }
        public double Price { get; set; }
        public bool IsASuggestion { get; set; }
        public string SuggestionImage { get; set; }
        public string SuggestionMessage = "";
        public string SuggestionDescription { get; set; }

    }
}
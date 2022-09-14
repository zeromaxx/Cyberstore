using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eshop.ViewModels
{
    public class FavouriteViewModel
    {
        public product Product { get; set; }
        public IEnumerable<product> Products { get; set; }
        public ILookup<int, favourite> Favourites { get; set; }

        public User User { get; set; }

        public User Admin { get; set; }

        public Boolean IsActive { get; set; }

        public IEnumerable<User> Admins { get; set; }

        public IEnumerable<User> Users { get; set; }


    }
}
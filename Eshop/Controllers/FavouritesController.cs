using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Eshop.Controllers
{
    public class FavouritesController : Controller
    {
        private Entities db = new Entities();
        // GET: Favourites
        public ActionResult Index()
        {            
            return View();
        }
        [HttpPost]
        public void AddFavouriteproduct(favourite favourite)
        {

            var userId = 0;
            if (User.Identity.Name != "")
            {
                userId = db.Users.SingleOrDefault(u => u.Email == User.Identity.Name).UserId;
            }
            favourite.userId = (int)userId;

            db.favourites.Add(favourite);
            db.SaveChanges();

        }

        [HttpDelete]
        public void DeleteFavouriteProduct(favourite favourite)
        {
            var userId = 0;
            if (User.Identity.Name != "")
            {
                userId = db.Users.SingleOrDefault(u => u.Email == User.Identity.Name).UserId;
            }


            var product = db.favourites.SingleOrDefault(a => a.userId == userId && a.ProductId == favourite.ProductId);

            db.favourites.Remove(product);
            db.SaveChanges();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
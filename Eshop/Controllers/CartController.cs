using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Migrations;
using PagedList;
using System.Net.Mail;
using System.Net;
using System.Data.Entity;
using Eshop.ViewModels;



namespace Eshop.Controllers
{
    public class CartController : Controller
    {
        private readonly Entities db;
        public CartController()
        {
            db = new Entities();
        }
        // GET: Cart
        public ActionResult Index()
        {
            if(!this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login","User");
            }
            var userId = 0;
            if (User.Identity.Name != "")
            {
                userId = db.Users.SingleOrDefault(u => u.Email == User.Identity.Name).UserId;
            }
            var cart = db.carts.Where(u => u.userId == userId).ToList();
            var total = db.carts
                .Where(p => p.userId == userId)
                .Select(p => p.product.price * p.quantity)
                .Sum() ?? 0;

            var thisUser = db.Users.SingleOrDefault(x => x.Email == this.User.Identity.Name);  // Allagh JJJ
            var desktopPrice = db.carts.
                Where(x => x.userId == userId).
                Select(y => y.Desktop.price)
                .Sum() ?? 0;
                


       
            var TotalPrice = total + desktopPrice;

            TempData["total"] = TotalPrice;                                                     // Allagh JJJ
            TempData["productsInCart"] = db.carts.Where(u => u.userId == userId).Count();
            return View(cart);
        }

        [HttpPost]
        public ActionResult AddToCartProduct(int id)
        {
            var userId = 0;
            if (User.Identity.Name != "")
            {
                userId = db.Users.SingleOrDefault(u => u.Email == User.Identity.Name).UserId;
            }

            var products = new cart()
            {
                quantity = 1,
                userId = (int)userId,
                productId = id
            };

            if (db.carts.Any(p => p.productId == id && p.userId == userId))
                return Json(new { status = "Product already added to cart" },JsonRequestBehavior.AllowGet);

            db.carts.Add(products);
            db.SaveChanges();

            return Json(new { status = "Added to Cart!" }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult AddDesktopToCart()
        {
            var userId = 0;
            if (User.Identity.Name != "")
            {
                userId = db.Users.SingleOrDefault(u => u.Email == User.Identity.Name).UserId;
            }
            var thisUser = db.Users.SingleOrDefault(x => x.Email == this.User.Identity.Name);

            var products = new cart()
            {
                quantity = 1,
                userId = (int)userId,
                desktopID = thisUser.MyDesktop
            };

            db.carts.Add(products);
            db.SaveChanges();

            return Json(new { status = "Added to Cart!" }, JsonRequestBehavior.AllowGet);

        }

        [HttpDelete]
        public void RemoveProductFromCart(int id)
        {
            var userId = 0;
            if (User.Identity.Name != "")
            {
                userId = db.Users.SingleOrDefault(u => u.Email == User.Identity.Name).UserId;
            }
          
            var cartInDb = db.carts.SingleOrDefault(c => c.Id == id && c.userId == userId);
            db.carts.Remove(cartInDb);
            db.SaveChanges();
        }

        public JsonResult UpdateCart(int prod_id, int prod_qty)
        {
            var userId = 0;
            if (User.Identity.Name != "")
            {
                userId = db.Users.SingleOrDefault(u => u.Email == User.Identity.Name).UserId;
            }
            var updateCart =
                db.carts.SingleOrDefault
                (p => p.userId == userId && p.productId == prod_id);

            updateCart.quantity = prod_qty;


            db.carts.AddOrUpdate(updateCart);
            db.SaveChanges();

            return Json(new { productId = prod_id, productQuantity = prod_qty }, JsonRequestBehavior.AllowGet);
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Eshop;
using Eshop.ViewModels;

namespace Eshop.Controllers
{

    public class OrdersController : Controller
    {
       
        private Entities db;
        public OrdersController()
        {
            db = new Entities();
        }

        public ActionResult MyOrders()
        {
            var userId = 0;
            if (User.Identity.Name != "")
            {
                userId = db.Users.SingleOrDefault(u => u.Email == User.Identity.Name).UserId;
            }
            var orders = db.orders.Where(u => u.userId == userId).ToList();
            
            TempData["productsInCart"] = db.carts.Where(x => x.userId == userId).Count();

            return View(orders);
        }

        public ActionResult MyOrder(int orderId)
        {
            var userId = 0;
            if (User.Identity.Name != "")
                userId = db.Users.SingleOrDefault(u => u.Email == User.Identity.Name).UserId;

            var myOrder = db.orders.SingleOrDefault(o => o.id == orderId);
            TempData["productsInCart"] = db.carts.Where(u => u.userId == userId).Count();
            return View("MyOrder", myOrder);
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

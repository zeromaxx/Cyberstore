using Eshop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Migrations;

namespace Eshop.Controllers
{
    public class DesktopController : Controller //JJJ
    {
        static List<product> SelectedProducts = new List<product>();
        static string type = "";
        private readonly Entities db;

        public DesktopController()
        {
            db = new Entities();
        }
        public ActionResult ChooseProduct(int id ,string action="")
        {

            switch (id)
            {
                case 1: SelectedProducts = db.products.Where(x => x.categoryId == 2).ToList(); var cpus = db.cpus.ToList(); type = "CPU"; break;
                case 2: SelectedProducts = db.products.Where(x => x.categoryId == 1).ToList(); var motherboards = db.motherboards.ToList(); type = "MotherBoards"; break;
                case 3: SelectedProducts = db.products.Where(x => x.categoryId == 3).ToList(); var gpus = db.gpus.ToList(); type = "GPU (Graphic Cards)"; break;
                case 4: SelectedProducts = db.products.Where(x => x.categoryId == 4).ToList(); var psus = db.psus.ToList(); type = "PSU"; break;
                case 5: SelectedProducts = db.products.Where(x => x.categoryId == 5).ToList(); var rams = db.rams.ToList(); type = "RAM"; break;
                case 6: SelectedProducts = db.products.Where(x => x.categoryId == 6).ToList(); var hardDiscs = db.hardDiscs.ToList(); type = "Hard Discs"; break;
            }
            var viewModel = new ProductViewModel
            {
                Products = SelectedProducts,    // static list Dilomenh sthn arxh
                Type = type                    // static string 
            };

            return View(viewModel);

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult RemoveFromDesktop(int id)
        {
            var thisUser = db.Users.SingleOrDefault(x => x.Email == this.User.Identity.Name);
            var thisDesktop = thisUser.Desktop;
            var _nullProduct = db.products.SingleOrDefault(y => y.categoryId == 7);

            if (thisDesktop.product.id == id) thisDesktop.product = _nullProduct;
            else if (thisDesktop.product1.id == id) thisDesktop.product1 = _nullProduct;
            else if (thisDesktop.product2.id == id) thisDesktop.product2 = _nullProduct;
            else if (thisDesktop.product3.id == id) thisDesktop.product3 = _nullProduct;
            else if (thisDesktop.product4.id == id) thisDesktop.product4 = _nullProduct;
            else if (thisDesktop.product5.id == id) thisDesktop.product5 = _nullProduct;

            db.Desktops.AddOrUpdate(thisDesktop);
            db.SaveChanges();
            return RedirectToAction("YourDesktop","Home");


        }
    }
}
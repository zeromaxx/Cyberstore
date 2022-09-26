using Eshop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Migrations;
using Eshop.Models;

namespace Eshop.Controllers
{
    public class DesktopController : Controller 
    {
        static List<product> SelectedProducts = new List<product>();
        static User ThisUser;
        private readonly Entities db;

        public DesktopController()
        {
            db = new Entities();
        }
      
        public ActionResult YourDesktop(string msg = "", int id = 0, string desc ="")
        {

            LoggedInUser();

            Desktop thisDesktop = ThisUser.Desktop;
            DesktopViewModel deskViewModel = new DesktopViewModel()
            {
                ProdMotherboards = db.products.Where(x => x.categoryId == 1 && x.stock > 0),
                ProdCpus = db.products.Where(x => x.categoryId == 2 && x.stock > 0),
                ProdGpus = db.products.Where(x => x.categoryId == 3 && x.stock > 0),
                ProdPsus = db.products.Where(x => x.categoryId == 4 && x.stock > 0),
                ProdRams = db.products.Where(x => x.categoryId == 5 && x.stock > 0),
                ProdHardDiscs = db.products.Where(x => x.categoryId == 6 && x.stock > 0),
                ProdBoxes = db.products.Where(x => x.categoryId == 9 && x.stock > 0),

                Cpus = db.cpus,
                Motherboards = db.motherboards,

            };
            if (id == 0)
            {
                thisDesktop.UpdateMyPrice();
                deskViewModel.IsASuggestion = false;
                deskViewModel.desktop = thisDesktop;

                db.Desktops.AddOrUpdate(thisDesktop);
                db.SaveChanges();
            }
            else if (id != 0)
            {
                var suggestedDesktop = db.Desktops.SingleOrDefault(x => x.ID == id);
                suggestedDesktop.UpdateMyPrice();

                ThisUser.Desktop.product = suggestedDesktop.product;
                ThisUser.Desktop.product1 = suggestedDesktop.product1;
                ThisUser.Desktop.product2 = suggestedDesktop.product2;
                ThisUser.Desktop.product3 = suggestedDesktop.product3;
                ThisUser.Desktop.product4 = suggestedDesktop.product4;
                ThisUser.Desktop.product5 = suggestedDesktop.product5;
                ThisUser.Desktop.product6 = suggestedDesktop.product6;
                ThisUser.Desktop.price = suggestedDesktop.price;

                switch (id)
                {
                    case 1027: deskViewModel.SuggestionImage = "sd1.jpeg"; break;
                    case 1028: deskViewModel.SuggestionImage = "sd2.jpeg"; break;
                    case 1029: deskViewModel.SuggestionImage = "sd2.jpeg"; break;
                    case 1031: deskViewModel.SuggestionImage = "sd4.jpeg"; break;
                    case 1032: deskViewModel.SuggestionImage = "sd5.jpeg"; break;
                    case 1033: deskViewModel.SuggestionImage = "sd6.jpeg"; break;
                    case 1034: deskViewModel.SuggestionImage = "sd7.jpeg"; break;
                    case 1035: deskViewModel.SuggestionImage = "sd8.jpeg"; break;
                    case 1036: deskViewModel.SuggestionImage = "sd9.jpeg"; break;
                }

                db.Desktops.AddOrUpdate(ThisUser.Desktop);
                db.SaveChanges();

                deskViewModel.SuggestionDescription = desc;
                deskViewModel.IsASuggestion = true;
                deskViewModel.desktop = suggestedDesktop;
                deskViewModel.SuggestionMessage = msg;


            }
            TempData["productsInCart"] = db.carts.Where(x => x.userId == ThisUser.UserId).Count();
            return View(deskViewModel);
        }
  
        public ActionResult RemoveFromDesktop(int id)
        {
            LoggedInUser();
           ;
            var thisDesktop = ThisUser.Desktop;
            var _nullProduct = db.products.SingleOrDefault(y => y.categoryId == 7);

            if (thisDesktop.product.id == id) thisDesktop.product = _nullProduct;
            else if (thisDesktop.product1.id == id) thisDesktop.product1 = _nullProduct;
            else if (thisDesktop.product2.id == id) thisDesktop.product2 = _nullProduct;
            else if (thisDesktop.product3.id == id) thisDesktop.product3 = _nullProduct;
            else if (thisDesktop.product4.id == id) thisDesktop.product4 = _nullProduct;
            else if (thisDesktop.product5.id == id) thisDesktop.product5 = _nullProduct;
            else if (thisDesktop.product6.id == id) thisDesktop.product6 = _nullProduct;

            thisDesktop.UpdateMyPrice();
            db.Desktops.AddOrUpdate(thisDesktop);
            db.SaveChanges();
            return RedirectToAction("YourDesktop","Desktop");
        }
        public void SendToDesktop(int id)
        {
            LoggedInUser();
            
            var product = db.products.SingleOrDefault(x => x.id == id);
            var desktop = ThisUser.Desktop;

            switch (product.categoryId)
            {
                case 1: desktop.MOTHERBOARD = product.id; desktop.product1 = product; break;
                case 2: desktop.CPU = product.id; desktop.product = product; break;
                case 3: desktop.GPU = product.id; desktop.product2 = product; break;
                case 4: desktop.PSU = product.id; desktop.product4 = product; break;
                case 5: desktop.RAM = product.id; desktop.product5 = product; break;
                case 6: desktop.HARDDISC = product.id; desktop.product3 = product; break;
                case 9: desktop.Box = product.id; desktop.product6 = product; break;
            }
            desktop.UpdateMyPrice();
            db.Desktops.AddOrUpdate(desktop);
            db.SaveChanges();
        }

        public ActionResult Suggestions()
        {

            LoggedInUser();
            TempData["productsInCart"] = db.carts.Where(x => x.userId == ThisUser.UserId).Count();
            Suggestion sug = new Suggestion();
            return View(sug);
        }
        [HttpPost]
        public ActionResult Suggestions(FormCollection formCollection)
        {

            bool Option_1 = false;
            bool Option_2 = false;
            bool Option_3 = false;
            bool Option_4 = false;
            bool Option_5 = false;
            string option = "";

            if (!string.IsNullOrEmpty(formCollection["Option_1"])) { Option_1 = true; }
            if (!string.IsNullOrEmpty(formCollection["Option_2"])) { Option_2 = true; }
            if (!string.IsNullOrEmpty(formCollection["Option_3"])) { Option_3 = true; }
            if (!string.IsNullOrEmpty(formCollection["Option_4"])) { Option_4 = true; }
            if (!string.IsNullOrEmpty(formCollection["Option_5"])) { Option_5 = true; }

            if (Option_1) { option = formCollection["Option_1"]; }
            if (Option_2) { option = formCollection["Option_2"]; }
            if (Option_3) { option = formCollection["Option_3"]; }
            if (Option_4) { option = formCollection["Option_4"]; }
            if (Option_5) { option = formCollection["Option_5"]; }

            var Money = Convert.ToDouble(formCollection["money"]);
            var price = 0;
            var _msg = "";
            var _id = 0;
            var _desc = "";

            if (Money < 300)
            {
                Suggestion sug = new Suggestion()
                {
                    money = Money,

                };
                return View(sug);
            }

            switch (option)
            {
                case "Simple use or future upgrade":
                    if (Money >= 300 && Money <= 400)  // id 2027 price 356 (300 - 400)
                    {
                        price = 356;
                        if (Money < 340)
                        {
                            _msg = Tools.Tools.MsgBuilder(price, Money);
                        }
                        _id = 1027;
                        _desc = Tools.Tools.SuggestedDesktopDescription(_id);
                    }
                    else if (Money > 400 && Money < 525) //id 1028 price 458  (401-524)
                    {
                        price = 458;
                        if (Money < 430)
                        {
                            _msg = Tools.Tools.MsgBuilder(price, Money);
                        }
                        _id = 1028;
                        _desc = Tools.Tools.SuggestedDesktopDescription(_id);
                    }
                    else if (Money >= 525)  //id 1029 price 588  (525-630)
                    {
                        price = 588;
                        if (Money < 570)
                        {
                            _msg = Tools.Tools.MsgBuilder(price, Money);
                        }
                        if (Money > 630)
                        {
                            _msg = Tools.Tools.MsgOverRun(price, Money);
                        }
                        _id = 1029;
                        _desc = Tools.Tools.SuggestedDesktopDescription(_id);
                    }
                    break;

                case "Gaming built 1080p-1440p-UHD-4K":
                    
                    if(Money < 800)
                    {
                        _msg = "We couldnt find a composition with " + Money + " $ in This Category.This is an option with 980 $ which is the cheepest one";
                        _id = 1031;
                    }

                    if (Money >= 800 && Money<900)
                    {
                        price = 980;
                        _msg = Tools.Tools.MsgBuilder(price, Money);
                        _id = 1031;
                        _desc = Tools.Tools.SuggestedDesktopDescription(_id);
                    }
                    if (Money >= 900 && Money < 1050) // id 1031 price 980 (900-1050)
                    {
                        price = 980;
                        if (Money < 950)
                        {
                            _msg = Tools.Tools.MsgBuilder(price, Money);
                        }

                        _id = 1031;
                        _desc = Tools.Tools.SuggestedDesktopDescription(_id);
                    }
                    else if (Money >= 1050 && Money <1250) // id = 1032 price 1147 (1050 -1200)
                    {
                        price = 1147;
                        if (Money < 1100)
                        {
                            _msg = Tools.Tools.MsgBuilder(price, Money);
                        }                   
                        _id = 1032;
                        _desc = Tools.Tools.SuggestedDesktopDescription(_id);
                    }
                    else if (Money >= 1250)
                    {
                        price = 1364;
                        if (Money < 1300)
                        {
                            _msg = Tools.Tools.MsgBuilder(price, Money);
                        }
                                            
                        _id = 1033;
                        _desc = Tools.Tools.SuggestedDesktopDescription(_id);
                    }break;
                    
                case "Workstation":
                    if (Money < 550 && Money <650)
                    {
                        price = 590;
                        _msg = Tools.Tools.MsgBuilder(price, Money);
                        _id = 1034;
                        _desc = Tools.Tools.SuggestedDesktopDescription(_id);
                    }
                    else if(Money >= 650 && Money<900)
                    {
                        price = 833;
                        if(Money<=750)
                        {
                            _msg = Tools.Tools.MsgBuilder(price, Money);
                        }
                        _id = 1035;
                        _desc = Tools.Tools.SuggestedDesktopDescription(_id);

                    }
                    else if(Money>900)
                    {
                        _id = 1036;
                        _desc = Tools.Tools.SuggestedDesktopDescription(_id);
                    }
                    ; break;
                case "Monster workstation":; break;

            }
            return RedirectToAction("YourDesktop", new { msg = _msg, id = _id , desc =_desc });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public void LoggedInUser()
        {
            ThisUser = db.Users.SingleOrDefault(x => x.Email == this.User.Identity.Name);
           
        }
    }
}
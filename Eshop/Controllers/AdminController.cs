using Eshop.ViewModels;
using Eshop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.IO;


namespace Eshop.Controllers
{
    [Authorize(Roles="Admin")]
    public class AdminController : Controller
    {
        private readonly Entities _context;
        public AdminController()
        {
            _context = new Entities();
        }

        static int ProductID;
        public ActionResult ViewAllProducts(int? page, string sortBy, int? categoryId, string livesearchtags)
        {
            var products = _context.products.AsQueryable();
            if (categoryId != null)
                products = _context.products.Where(p => p.categoryId == categoryId);

            if (!string.IsNullOrEmpty(livesearchtags))
            {
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] words = livesearchtags.Split(delimiterChars);
                products = products.Where(p => words.Any(w => p.Description.Contains(w) ||
                                                              p.category.name.Contains(w) ||
                                                              p.Name.Contains(w)
                                          ));
            }
            switch (sortBy)
            {
                case "Price asc": products = products.OrderBy(x => x.price); break;
                case "Price desc":products = products.OrderByDescending(x => x.price); break;
                case "newness": products = products.OrderByDescending(x => x.createdAt); break;
                case "name": products = products.OrderBy(x => x.Name); break;
                case "200":  products = products.OrderBy(x => x.Name); products = products.Where(x => x.price <= 200); break;
                case "400": products = products.OrderBy(x => x.Name); products = products.Where(x => x.price > 200 && x.price <= 400); break;
                case "600": products = products.OrderBy(x => x.Name); products = products.Where(x => x.price > 400 && x.price <= 600); break;
                case "800": products = products.OrderBy(x => x.Name); products = products.Where(x => x.price > 600 && x.price <= 800); break;
                case "top": products = products.OrderBy(x => x.Name); products = products.Where(x => x.price > 800); break;
                default: products = products.OrderBy(x => x.Name); break;
            }

            ViewBag.categoryId = categoryId;
            ViewBag.sortBy = sortBy;
            return View("ViewAllProducts", products.ToPagedList(page ?? 1, 6));
        }
        public ActionResult Index()
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
            TempData["Username"] = user.Username;
            return View();
        }
        public ActionResult GetProductData()
        {
            var productsUnder200 = _context.products
                .Where(p => p.price < 200)
                .Count();
            var productsBetween200And600 = _context.products
                .Where(p => p.price > 200 && p.price <= 600)
                .Count();
            var productsover600 = _context.products
                .Where(p => p.price > 600)
                .Count();

            var products = new
            {
                _productsUnder200 = productsUnder200,
                _productsBetween200And600 = productsBetween200And600,
                _productsover600 = productsover600
            };
           
            return Json(products, JsonRequestBehavior.AllowGet);            
        }

        public ActionResult GetProductSaleRate()
        {
            
            var top5ProductNames = (from item in _context.products
                           group item.sales by item.Name
                           into g orderby g.Sum() descending
                           select g.Key).Take(8).ToList();

            var top5ProductSales = _context.products
               .OrderByDescending(p => p.sales)
               .Take(8).Select(p => p.sales).ToList();

            var products = new
            {
                Products = top5ProductNames,
                Top5ProductSales = top5ProductSales
            };

            return Json(products, JsonRequestBehavior.AllowGet);         
        }

        public ActionResult GetFrequentCustomers()
        {
          
            _context.Configuration.ProxyCreationEnabled = false;

            var top5CustomersInOrders = (from item in _context.orders
                                         join u in _context.Users
                                         on item.userId equals u.UserId
                                         group item.id by u.Username
                                         into g
                                         orderby g.Count() descending
                                         select g.Key).ToList();

            var top5OrderId = (from item in _context.orders
                                         group item.id by item.userId
                                       into g
                                         orderby g.Count() descending
                                         select g.Count()).ToList();

            var userData = new
            {
                Top5CustomersInOrders = top5CustomersInOrders,
                Top5OrderId = top5OrderId
            };
            return Json(userData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult InsertProduct(int id)
        {
            ProductViewModel viewModel = new ProductViewModel();

            switch (id)
            {
                case 1: viewModel._string = "~/Views/Admin/PartialViews/_MotherboardPartial.cshtml"; viewModel.CategoryID = 1;viewModel.Type = "Insert a new Motherboard ";viewModel.img = "MSIMotherboardZ490ACE_3.jpg"; break;
                case 2: viewModel._string = "~/Views/Admin/PartialViews/_CpuPartial.cshtml"; viewModel.CategoryID = 2; viewModel.Type = "Insert a new CPU "; viewModel.img ="InsertCPU.jpg"; break;
                case 3: viewModel._string = "~/Views/Admin/PartialViews/_GpuPartial.cshtml"; viewModel.CategoryID = 3; viewModel.Type = "Insert a new GPU (Graphic card) "; viewModel.img = "g4.jpeg"; break;
                case 4: viewModel._string = "~/Views/Admin/PartialViews/_PsuPartial.cshtml"; viewModel.CategoryID = 4; viewModel.Type = "Insert a new PSU "; viewModel.img = "ps1.jpeg"; break;
                case 5: viewModel._string = "~/Views/Admin/PartialViews/_RamPartial.cshtml"; viewModel.CategoryID = 5; viewModel.Type = "Insert a new RAM "; viewModel.img ="InsertRAM.jpg";break;
                case 6: viewModel._string = "~/Views/Admin/PartialViews/_HardDiskPartial.cshtml"; viewModel.CategoryID = 6; viewModel.Type = "Insert a new Hard Disc "; break;
                case 8: viewModel._string = "~/Views/Admin/PartialViews/_MonitorPartial.cshtml";viewModel.CategoryID = 8;viewModel.Type = "Insert a new monitor";break;
                case 9: viewModel._string = "~/Views/Admin/PartialViews/_BoxPartial.cshtml"; viewModel.CategoryID = 9; viewModel.Type = "Insert a new box"; break; //GGGGG partial
            }
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsertProduct(ProductViewModel viewModel)
        {        
            if (viewModel.Image1 != null) { viewModel.Product.image1 = Path.GetFileName(viewModel.Image1.FileName); string fullPath = Path.Combine(Server.MapPath("~/Images/"), viewModel.Product.image1); viewModel.Image1.SaveAs(fullPath); }
            if (viewModel.Image2 != null) { viewModel.Product.image2 = Path.GetFileName(viewModel.Image2.FileName); string fullPath = Path.Combine(Server.MapPath("~/Images/"), viewModel.Product.image2); viewModel.Image2.SaveAs(fullPath); }
            if (viewModel.Image3 != null) { viewModel.Product.image3 = Path.GetFileName(viewModel.Image3.FileName); string fullPath = Path.Combine(Server.MapPath("~/Images/"), viewModel.Product.image3); viewModel.Image3.SaveAs(fullPath); }
            if (viewModel.Thumbnail != null) { viewModel.Product.thumbnail = Path.GetFileName(viewModel.Thumbnail.FileName); string fullPath = Path.Combine(Server.MapPath("~/Images/"), viewModel.Product.thumbnail); viewModel.Thumbnail.SaveAs(fullPath); }

            var _product = viewModel.Product;
            _product.categoryId = viewModel.CategoryID;
            _product.createdAt = DateTime.Now;

            _context.products.AddOrUpdate(_product);
            _context.SaveChanges();

            switch (viewModel.CategoryID)
            {
                case 1:
                    var _motherboard = viewModel.Motherboard;
                    ProductID = _context.products.SingleOrDefault(p => p.id == viewModel.Product.id).id;
                    _motherboard.productId = ProductID;

                    _context.motherboards.AddOrUpdate(_motherboard);
                    _context.SaveChanges(); break;

                case 2:
                    var _cpu = viewModel.Cpu;
                    ProductID = _context.products.SingleOrDefault(p => p.id == viewModel.Product.id).id;
                    _cpu.productId = ProductID;

                    _context.cpus.AddOrUpdate(_cpu);
                    _context.SaveChanges(); break;

                case 3:
                    var _gpu = viewModel.Gpu;
                    ProductID = _context.products.SingleOrDefault(p => p.id == viewModel.Product.id).id;
                    _gpu.productId = ProductID;

                    _context.gpus.AddOrUpdate(_gpu);
                    _context.SaveChanges(); break;

                case 4:
                    var _psu = viewModel.Psu;
                    ProductID = _context.products.SingleOrDefault(p => p.id == viewModel.Product.id).id;
                    _psu.productId = ProductID;

                    _context.psus.AddOrUpdate(_psu);
                    _context.SaveChanges(); break;

                case 5:
                    var _ram = viewModel.Ram;
                    ProductID = _context.products.SingleOrDefault(p => p.id == viewModel.Product.id).id;
                    _ram.productId = ProductID;

                    _context.rams.AddOrUpdate(_ram);
                    _context.SaveChanges(); break;

                case 6:
                    var _hardDisc = viewModel.HardDisc;
                    ProductID = _context.products.SingleOrDefault(p => p.id == viewModel.Product.id).id;
                    _hardDisc.productId = ProductID;

                    _context.hardDiscs.AddOrUpdate(_hardDisc);
                    _context.SaveChanges(); break;
                case 8:
                    var _monitor = viewModel.Monitor;
                    ProductID = _context.products.SingleOrDefault(p => p.id == viewModel.Product.id).id;
                    _monitor.productId = ProductID;

                    _context.monitors.AddOrUpdate(_monitor);
                    _context.SaveChanges(); break;
                case 9:
                    var _box = viewModel.Box;
                    ProductID = _context.products.SingleOrDefault(p => p.id == viewModel.Product.id).id;
                    _box.productId = ProductID;

                    _context.boxes.AddOrUpdate(_box);
                    _context.SaveChanges(); break;
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
           
            ProductViewModel viewModel = new ProductViewModel()
            {
                Product = _context.products.SingleOrDefault(p => p.id == id)
            };

            switch (viewModel.Product.categoryId)
            {
                case 1: viewModel._string = "~/Views/Admin/PartialViews/_MotherboardPartial.cshtml"; viewModel.Motherboard = _context.motherboards.SingleOrDefault(x => x.productId == id);viewModel.CategoryID = 1;viewModel.ItemID = viewModel.Motherboard.id; break;
                case 2: viewModel._string = "~/Views/Admin/PartialViews/_CpuPartial.cshtml"; viewModel.Cpu = _context.cpus.SingleOrDefault(x => x.productId == id); viewModel.CategoryID = 2; viewModel.ItemID = viewModel.Cpu.id; break;
                case 3: viewModel._string = "~/Views/Admin/PartialViews/_GpuPartial.cshtml"; viewModel.Gpu = _context.gpus.SingleOrDefault(x => x.productId == id); viewModel.CategoryID = 3; viewModel.ItemID = viewModel.Gpu.id; break;
                case 4: viewModel._string = "~/Views/Admin/PartialViews/_PsuPartial.cshtml"; viewModel.Psu = _context.psus.SingleOrDefault(x => x.productId == id); viewModel.CategoryID = 4; viewModel.ItemID = viewModel.Psu.id; break;
                case 5: viewModel._string = "~/Views/Admin/PartialViews/_RamPartial.cshtml"; viewModel.Ram = _context.rams.SingleOrDefault(x => x.productId == id); viewModel.CategoryID = 5; viewModel.ItemID = viewModel.Ram.id; break;
                case 6: viewModel._string = "~/Views/Admin/PartialViews/_HardDiskPartial.cshtml"; viewModel.HardDisc = _context.hardDiscs.SingleOrDefault(x => x.productId == id); viewModel.CategoryID = 6; viewModel.ItemID = viewModel.HardDisc.id; break;
                case 8: viewModel._string = "~/Views/Admin/PartialViews/_MonitorPartial.cshtml"; viewModel.Monitor = _context.monitors.SingleOrDefault(x => x.productId == id); viewModel.CategoryID = 8; viewModel.ItemID = viewModel.Monitor.ID; break;
                case 9: viewModel._string = "~/Views/Admin/PartialViews/_BoxPartial.cshtml"; viewModel.Box = _context.boxes.SingleOrDefault(x => x.productId == id); viewModel.CategoryID = 9; viewModel.ItemID = viewModel.Box.id; break;
            }
            
            viewModel.ProductID = id;
            
            return View("Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {

                return View("Edit", viewModel);
            }
            if (viewModel.Image1 != null) { viewModel.Product.image1 = Path.GetFileName(viewModel.Image1.FileName); string fullPath = Path.Combine(Server.MapPath("~/Images/"), viewModel.Product.image1); viewModel.Image1.SaveAs(fullPath); }
            if (viewModel.Image2 != null) { viewModel.Product.image2 = Path.GetFileName(viewModel.Image2.FileName); string fullPath = Path.Combine(Server.MapPath("~/Images/"), viewModel.Product.image2); viewModel.Image2.SaveAs(fullPath); }
            if (viewModel.Image3 != null) { viewModel.Product.image3 = Path.GetFileName(viewModel.Image3.FileName); string fullPath = Path.Combine(Server.MapPath("~/Images/"), viewModel.Product.image3); viewModel.Image3.SaveAs(fullPath); }
            if (viewModel.Thumbnail != null) { viewModel.Product.thumbnail = Path.GetFileName(viewModel.Thumbnail.FileName); string fullPath = Path.Combine(Server.MapPath("~/Images/"), viewModel.Product.thumbnail); viewModel.Thumbnail.SaveAs(fullPath); }


            var product = viewModel.Product;
            product.id = viewModel.ProductID;
            product.categoryId = viewModel.CategoryID;
            product.createdAt = DateTime.Now;
          
            _context.products.AddOrUpdate(product);
            _context.SaveChanges();

            switch (viewModel.Product.categoryId)
            {
                case 1: viewModel.Motherboard.productId = viewModel.Product.id;viewModel.Motherboard.id=viewModel.ItemID ; _context.motherboards.AddOrUpdate(viewModel.Motherboard); _context.SaveChanges(); break;
                case 2: viewModel.Cpu.productId = viewModel.Product.id; viewModel.Cpu.id = viewModel.ItemID; _context.cpus.AddOrUpdate(viewModel.Cpu); _context.SaveChanges(); break;
                case 3: viewModel.Gpu.productId = viewModel.Product.id; viewModel.Gpu.id = viewModel.ItemID; _context.gpus.AddOrUpdate(viewModel.Gpu); _context.SaveChanges(); break;
                case 4: viewModel.Psu.productId = viewModel.Product.id; viewModel.Psu.id = viewModel.ItemID; _context.psus.AddOrUpdate(viewModel.Psu); _context.SaveChanges(); break;
                case 5: viewModel.Ram.productId = viewModel.Product.id; viewModel.Ram.id = viewModel.ItemID; _context.rams.AddOrUpdate(viewModel.Ram); _context.SaveChanges(); break;
                case 6: viewModel.HardDisc.productId = viewModel.Product.id; viewModel.HardDisc.id = viewModel.ItemID; _context.hardDiscs.AddOrUpdate(viewModel.HardDisc); _context.SaveChanges(); break;
                case 8: viewModel.Monitor.productId = viewModel.Product.id; viewModel.Monitor.ID = viewModel.ItemID; _context.monitors.AddOrUpdate(viewModel.Monitor); _context.SaveChanges(); break;
                case 9: viewModel.Box.productId = viewModel.Product.id; viewModel.Box.id = viewModel.ItemID; _context.boxes.AddOrUpdate(viewModel.Box); _context.SaveChanges(); break;
            }
           

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int? id)
        {

            var product = _context.products.Single(p => p.id == id);
         
            switch (product.categoryId)
            {
                case 1: _context.motherboards.Remove(_context.motherboards.SingleOrDefault(x => x.productId == product.id)); _context.SaveChanges(); break;
                case 2: _context.cpus.Remove(_context.cpus.SingleOrDefault(x => x.productId == product.id)); _context.SaveChanges(); break;
                case 3: _context.gpus.Remove(_context.gpus.SingleOrDefault(x => x.productId == product.id)); _context.SaveChanges(); break;
                case 4: _context.psus.Remove(_context.psus.SingleOrDefault(x => x.productId == product.id)); _context.SaveChanges(); break;
                case 5: _context.rams.Remove(_context.rams.SingleOrDefault(x => x.productId == product.id)); _context.SaveChanges(); break;
                case 6: _context.hardDiscs.Remove(_context.hardDiscs.SingleOrDefault(x => x.productId == product.id)); _context.SaveChanges(); break;
                case 8: _context.monitors.Remove(_context.monitors.SingleOrDefault(x => x.productId == product.id)); _context.SaveChanges(); break;
                case 9: _context.boxes.Remove(_context.boxes.SingleOrDefault(x => x.productId == product.id)); _context.SaveChanges(); break;

            }
            _context.products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult EditRole(int? id)
        {
            var userId = _context.UserRoleMappings
                .SingleOrDefault(u => u.UserId == id);

            switch (userId.RoleId)
            {
                case 1:
                    userId.RoleId = 2;
                    _context.SaveChanges();
                    break;
                case 2:
                    userId.RoleId = 1;
                    _context.SaveChanges();
                    break;
            }
            return RedirectToAction("CreateRole");
        }
        public ActionResult CreateRole(int?page,string category,string livesearchtags)
        {
            var userRoles = _context.UserRoleMappings
                .Include(u => u.User)
                .Include(u => u.Role)
                .AsQueryable();


            if (!string.IsNullOrEmpty(livesearchtags))
            {
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] words = livesearchtags.Split(delimiterChars);
                userRoles = userRoles.Where(u => words.Any(w => u.User.address.Contains(w) ||
                                                              u.User.Email.Contains(w) ||
                                                              u.User.LastName.Contains(w) ||
                                                              u.User.FirstName.Contains(w) ||
                                                              u.User.Phone.Contains(w) ||
                                                              u.User.Username.Contains(w) ||
                                                              u.Role.RoleName.Contains(w)
                                          ));
            }



            switch (category)
            {
                case "Admin":
                    userRoles = userRoles.Where(u => u.RoleId == 1);
                    break;
                case "Customer":
                    userRoles = userRoles.Where(u => u.RoleId == 2);
                    break;
            }



            userRoles = userRoles.OrderBy(x => x.User.LastName);
            var userViewModel = new UserRoleViewModel()
            {
                UserRoleMappings = userRoles.ToPagedList(page ?? 1, 6)
            };
            return View("CreateRole", userViewModel);
        }          
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
     
    }
}
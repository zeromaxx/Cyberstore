using System.Linq;
using System.Web.Mvc;
using PagedList;
using System.Net.Mail;
using System.Net;
using System.Data.Entity;
using Eshop.ViewModels;
using System;
using System.Web;
using System.Data.Entity.Migrations;
using System.Collections.Generic;
using Eshop.Models.Dtos;
using AutoMapper;
using Eshop.Models;
using Eshop.Tools;

namespace Eshop.Controllers
{
  
    public class HomeController : Controller
    {
        static int _productId = 0;
        static int _desktopId = 0;
        static decimal _price = 0;
        public Dictionary<string, string> emails;

        private readonly Entities db = new Entities(); //Readonly allagh JJJ
        public Connection connection;
        public HomeController()
        {
            db = new Entities();    //Constructor 
            connection = new Connection();
            emails = new Dictionary<string, string>
            {
                { "Aris", "zeromixer2010@yahoo.com" }
            };
        }

        public ActionResult Index()
        {
            var userId = 0;
            User user = null;
            if (User.Identity.Name != "")
            {
                var name = User.Identity.Name;
                userId = db.Users.SingleOrDefault(u => u.Email == User.Identity.Name).UserId;
                user = db.Users.SingleOrDefault(u => u.Email == User.Identity.Name);
                TempData["userId"] = userId;
            }
            var favouriteProducts = db.favourites
                .Where(u => u.userId == userId)
                .ToList()
                .ToLookup(u => u.ProductId);

            var addedCartProducts = db.carts
             .Where(u => u.userId == userId)
             .ToList()
             .ToLookup(u => u.productId);

            ViewBag.addedCartProducts = addedCartProducts;
            ViewBag.Favourites = favouriteProducts;

            var msgs = db.SupportMessages.ToList();
            TempData["productsInCart"] = db.carts.Where(u => u.userId == userId).Count();          
            var viewmodel = new FavouriteViewModel()
            {
                Products = db.products.ToList().Take(10),
                Favourites = favouriteProducts,
                User = user
            };
            return View("Index", viewmodel);

        }

        public ActionResult AboutUs()
        {

            return View();
        }

        public ActionResult Contact()
        {

            return View();
        }

      
       [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Contactform(ConnectionViewModel conn) //REFACTOR HERE - TEMPDATA NOT SENDING DATA TO VIEW
        {
            User userInDb = null;
            TempData["msg"] = "";
            if (!ModelState.IsValid)
                TempData["msg"] = "Something went wrong, please try again";
          
            MailMessage mail = new MailMessage();
            Dictionary<string, string>.ValueCollection EmailValues = emails.Values;

            foreach (var address in EmailValues)
            {
                mail.To.Add(address.ToString()); //new MailAddress
            }

                        
                userInDb = db.Users.FirstOrDefault(u => u.Email == conn.Email);

                mail.From = new MailAddress("finalproject20001@gmail.com");
                //add user name to some one
                mail.Subject = $"{conn.Email} Thank you for contacting our support";
                mail.Body = conn.Email +" "+ "Message: "+" " +conn.Inquiry;

                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Credentials = new System.Net.NetworkCredential
                         ("finalproject20001@gmail.com", "lmnanzdhnmlnzzpl"),
                    EnableSsl = true,
                    Port = 587
                };

                smtp.Send(mail);

                var contact = new Contactform()
                {
                    FirstName = conn.FirstName,
                    LastName = conn.LastName,
                    Email = conn.Email,
                    Enquiry = conn.Inquiry
                };
                db.Contactforms.Add(contact);
                db.SaveChanges();
                TempData["emailSentNotification"] = "Message Sent! We will contact you as soon as possible.";
                      

            return RedirectToAction("Contact");
        }
        
        public ActionResult Products(int? page, string sortBy, string range, int? categoryId)
        {       

            var products = db.products.AsQueryable();
            if (categoryId != null)
                products = db.products.Where(p => p.categoryId == categoryId);

            switch (range)
            {
                case "200":
                    products = products.Where(x => x.price <= 200);
                    break;
                case "400":
                    products = products.Where(x => x.price > 200 && x.price <= 400);
                    break;
                case "600":
                    products = products.Where(x => x.price > 400 && x.price <= 600);
                    break;
                case "800":
                    products = products.Where(x => x.price > 600 && x.price <= 800);
                    break;
                case "top":
                    products = products.Where(x => x.price > 800);
                    break;
                case "all":
                    break;

            }

            switch (sortBy)
            {
                case "Price asc":
                    products = products.OrderBy(x => x.price);
                    break;
                case "Price desc":
                    products = products.OrderByDescending(x => x.price);
                    break;
                case "newness":
                    products = products.OrderByDescending(x => x.createdAt);
                    break;
                case "name":
                    products = products.OrderBy(x => x.Name);
                    break;
                default:
                    products = products.OrderBy(x => x.Name);
                    break;
            }
            
            ViewBag.categoryId = categoryId;
            ViewBag.sortBy = sortBy;
            ViewBag.range = range;           

            var userId = 0;
            if (User.Identity.Name != "")
            {
                userId = db.Users.SingleOrDefault(u => u.Email == User.Identity.Name).UserId;
                TempData["userId"] = userId;
            }
            
            var favouriteProducts = db.favourites
                .Where(u => u.userId == userId)
                .ToList()
                .ToLookup(u => u.ProductId);
            var addedCartProducts = db.carts
              .Where(u => u.userId == userId)
              .ToList()
              .ToLookup(u => u.productId);

            ViewBag.addedCartProducts = addedCartProducts;
            ViewBag.Favourites = favouriteProducts;

            TempData["productsInCart"] = db.carts.Where(u => u.userId == userId).Count();
            return View("Products", products.ToPagedList(page ?? 1, 6)); ;
            
        }
        
        public ActionResult Shop(int? page)
        {
            return View(db.products.OrderBy(n => n.Name).ToPagedList(page ?? 1, 6));
        }

      
        public ActionResult ProductDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var product = db.products.SingleOrDefault(p => p.id == id);
            var cpu = db.cpus.SingleOrDefault(c => c.productId == product.id);
            var psu = db.psus.SingleOrDefault(c => c.productId == product.id);
            var gpu = db.gpus.SingleOrDefault(c => c.productId == product.id);
            var motherboard = db.motherboards.SingleOrDefault(c => c.productId == product.id);
            var hardDisk = db.hardDiscs.SingleOrDefault(c => c.productId == product.id);
            var ram = db.rams.SingleOrDefault(c => c.productId == product.id);
            var monitor = db.monitors.SingleOrDefault(c => c.productId == product.id);
            var box = db.boxes.SingleOrDefault(c => c.productId == product.id);

            var userId = 0;
            if (User.Identity.Name != "")
            {
                userId = db.Users.SingleOrDefault(u => u.Email == User.Identity.Name).UserId;
            }

            TempData["productsInCart"] = db.carts.Where(u => u.userId == userId).Count();
            var favouriteProducts = db.favourites
                .Where(u => u.userId == userId)
                .ToList()
                .ToLookup(u => u.ProductId);
            var addedCartProducts = db.carts
              .Where(u => u.userId == userId)
              .ToList()
              .ToLookup(u => u.productId);

            ViewBag.addedCartProducts = addedCartProducts;

            ViewBag.Favourites = favouriteProducts;

            if (product == null)
            {
                return HttpNotFound();
            }
            ProductViewModel viewModel = new ProductViewModel
            {
                Product = product,
                Cpu = cpu,
                Psu = psu,
                Gpu = gpu,
                Motherboard = motherboard,
                HardDisc = hardDisk,
                Ram = ram,
                Monitor = monitor,
                Box=box

            };
            return View(viewModel);
        }

        public ActionResult SendEmail()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendEmail(string receiver)
        {
            if (ModelState.IsValid)
            {
                var senderEmail = new MailAddress("finalproject20001@gmail.com", "CyberStore");
                var receiverEmail = new MailAddress(receiver, "Receiver");
                var password = "lmnanzdhnmlnzzpl";
                var subject = "Thank you for subscribing to our newsletter!";
                var body = "Hey , we hope to hear more from you soon!";
                var smtp = new SmtpClient
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password),
                    Host = "smtp.gmail.com",
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,

                };
                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(mess);
                }
                return Json(new { status = "Thank you for subscribing to our newsletter!" }, JsonRequestBehavior.AllowGet);
            }


            return Json(new { status = "Something went wrong, please try again" }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult WishList()
        {
            var userId = 0;
            if (User.Identity.Name != "")
            {
                userId = db.Users.SingleOrDefault(u => u.Email == User.Identity.Name).UserId;
            }
            
            TempData["productsInCart"] = db.carts.Where(u => u.userId == userId).Count();
            var products = db
            .favourites
            .Where(u => u.userId == userId).Include(u => u.product).ToList();
            return View(products);

        }


        public ActionResult LiveTagSearch(string search)
        {

            var res = (from t in db.products
                       where t.Name.Contains(search)
                       select t).ToList();

            if (!res.Any())
            {
                ViewBag.NoResults = "No results were found";
            }

            var userId = 0;
            if (User.Identity.Name != "")
            {
                userId = db.Users.SingleOrDefault(u => u.Email == User.Identity.Name).UserId;
            }
            TempData["productsInCart"] = db.carts.Where(u => u.userId == userId).Count();
            return PartialView(res);
        }

        //GENERATING ORDER TOKEN
        private static Random Random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public ActionResult Checkout()
        {
            var userId = 0;
            if (User.Identity.Name != "")
            {       
                
                userId = Me().UserId;
            }
            var cart = db.carts.Where(u => u.userId == userId);

            var totalProducts = db.carts
               .Where(p => p.userId == userId)
               .Select(p => p.product.price * p.quantity)
               .Sum() ?? 0;

            var thisUser = db.Users.SingleOrDefault(x => x.Email == this.User.Identity.Name);
            decimal? _totalPrice = totalProducts;

            foreach ( cart cartItem in cart)
            {
                if(cartItem.productId==null)
                {
                    var desktopPrice = thisUser.Desktop.price;
                     _totalPrice = totalProducts + desktopPrice;
                }
            }
           
           

            var viewModel = new OrderViewModel()
            {
                CartItems = cart.ToList(),
                TotalPrice = (int)_totalPrice,
                UserId = userId,
                CreatedAt = DateTime.Now,
                TrackingNumber = RandomString(14),
                ShippingId=0
                

            };
            TempData["productsInCart"] = db.carts.Where(u => u.userId == userId).Count();
            return View("Checkout", viewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Checkout(OrderViewModel model)
        {
          

            if (!ModelState.IsValid)
            {
                User thisUser = Me();               
                var desktopPrice = thisUser.Desktop.price;              
               
                var totalProducts = db.carts
                .Where(p => p.userId == thisUser.UserId)
                .Select(p => p.product.price * p.quantity)
                .Sum() ?? 0;
                
                var _totalPrice = totalProducts + desktopPrice ;
                
                var cart = db.carts.Where(u => u.userId == thisUser.UserId);

                var viewModel = new OrderViewModel()
                {                   
                    CartItems = cart.ToList(),
                    TotalPrice = (int)_totalPrice,
                    UserId = thisUser.UserId,
                    CreatedAt = DateTime.Now,
                    TrackingNumber = RandomString(14)
                };
               
                return View(model);
            }
     

            var user = db.Users.SingleOrDefault(us => us.UserId == model.UserId);
            var viewAdress = new OrderViewModel()
            {
                City = model.City,
                Street = model.Street,
                PostalCode = model.PostalCode
            };

            viewAdress.Adress();
            var Order = new order()
            {
                firstName = model.FirstName,
                lastName = model.LastName,
                userId = model.UserId,
                createdAt = DateTime.Now,
                phoneNumber = model.PhoneNumber,
                shippingId = model.ShippingId,
                trackingNumber = model.TrackingNumber
                ,
                address = viewAdress._address,
                paymentId = 2,
                price = model.TotalPrice + model.ShippingId //Convert.ToDecimal(model.ShippingId.)
            };
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.address = viewAdress._address;
            user.Phone = model.PhoneNumber;
            db.orders.Add(Order);
            db.SaveChanges();
            var cartItemsToDelete = db.carts.Where(u => u.userId == model.UserId).ToList();
            foreach (var cartItem in cartItemsToDelete)
            {
           
                
                if(cartItem.productId==null)
                {
               
                    _price = (decimal)cartItem.Desktop.price;
                    _desktopId = (int)cartItem.desktopID;

                    var OrderItem = new OrderItem()
                    {
                        orderId = Order.id,
                        price = _price,
                        quantity = cartItem.quantity,
                        desktopId = _desktopId
                    };


                    var thisUser = db.Users.SingleOrDefault(x => x.Email == this.User.Identity.Name);
                    var _nullProduct = db.products.SingleOrDefault(x => x.categoryId == 7);

                    if ( cartItem.Desktop.product.id!=64  ) Tools.Tools.SalesAndStock(cartItem.Desktop.product.id); thisUser.Desktop.product   = _nullProduct;
                    if (cartItem.Desktop.product1.id != 64) Tools.Tools.SalesAndStock(cartItem.Desktop.product1.id); thisUser.Desktop.product1 = _nullProduct;
                    if (cartItem.Desktop.product2.id != 64) Tools.Tools.SalesAndStock(cartItem.Desktop.product2.id); thisUser.Desktop.product2 = _nullProduct;
                    if (cartItem.Desktop.product3.id != 64) Tools.Tools.SalesAndStock(cartItem.Desktop.product3.id); thisUser.Desktop.product3 = _nullProduct;
                    if (cartItem.Desktop.product4.id != 64) Tools.Tools.SalesAndStock(cartItem.Desktop.product4.id); thisUser.Desktop.product4 = _nullProduct;
                    if (cartItem.Desktop.product5.id != 64) Tools.Tools.SalesAndStock(cartItem.Desktop.product5.id); thisUser.Desktop.product5 = _nullProduct;
                    if (cartItem.Desktop.product6.id != 64) Tools.Tools.SalesAndStock(cartItem.Desktop.product6.id); thisUser.Desktop.product6 = _nullProduct;
                    
                    thisUser.Desktop.UpdateMyPrice();

                    db.Desktops.AddOrUpdate(thisUser.Desktop);
                    db.OrderItems.Add(OrderItem);
                    db.carts.Remove(cartItem);
                    db.SaveChanges();
                }
                else if(cartItem.desktopID==null)
                {
                    
                    _price = (decimal)cartItem.product.price;
                    _productId= (int)cartItem.productId;
                    
                    var OrderItem = new OrderItem()
                    {
                        orderId = Order.id,
                        price = _price,
                        quantity = cartItem.quantity,
                        productId  = _productId


                    };
                    Tools.Tools.SalesAndStock(cartItem.productId,cartItem.quantity);
                    db.OrderItems.Add(OrderItem);
                    db.carts.Remove(cartItem);
                    db.SaveChanges();
                }                                                                                  
            }
   

            TempData["OrderComplete"] = "Your purchase was successful. Thank you!";
           
            return RedirectToAction("Checkout", "Home");
        }

         
        public ActionResult ProductComparsion(int id1, int id2 = 0,int id3=0,int id4=0)
        {
            var product = db.products.SingleOrDefault(x => x.id == id1);
            ComparsionViewModel Model = new ComparsionViewModel()
            {
                id1 = id1,
                id2 = id2,
                id3 = id3,
                id4 = id4
            };
            
            if (id2 == 0)
            {
                Model.MyList = GetMyList(product.categoryId);              
                Model.MyList.Remove(product);
            
                return View(Model);
            }
            if(id3==0)
            {
                Model.MyList = GetMyList(product.categoryId);                                     
                Model.MyList.Remove((db.products.SingleOrDefault(x => x.id == Model.id1)));
                Model.MyList.Remove((db.products.SingleOrDefault(x => x.id == Model.id2)));   
               
                return View(Model);
            }
            if (id4 == 0)
            {
                Model.MyList = GetMyList(product.categoryId);               
                Model.MyList.Remove((db.products.SingleOrDefault(x => x.id == Model.id1)));
                Model.MyList.Remove((db.products.SingleOrDefault(x => x.id == Model.id2)));
                Model.MyList.Remove((db.products.SingleOrDefault(x => x.id == Model.id3)));
               
                return View(Model);
            }

            return View();

        } // send the proper products list on user in order to choose some of them to compare with the basic
        public ActionResult ComparisonTable(int id1 ,int id2=0,int id3=0,int id4=0)
        {
           
            var _product = db.products.SingleOrDefault(x => x.id == id1);
            var nullProduct = Tools.Tools.GetNullProduct();
                 
            ComparsionViewModel viewModel = new ComparsionViewModel()
            {
                id1=id1,
                id2=id2,
                id3=id3,
                id4=id4,
                product2 = nullProduct,
                product3 = nullProduct,
                product4 = nullProduct
            };
            switch (_product.categoryId)
            {
                case 1: _product.thisMotherboard = db.motherboards.SingleOrDefault(x => x.productId == id1); viewModel.Partial = "~/Views/Home/PartialViews/_comparisonMotherboard.cshtml"; break;
                case 2: _product.thisCpu = db.cpus.SingleOrDefault(x => x.productId == id1); viewModel.Partial = "~/Views/Home/PartialViews/_comparisonCPU.cshtml"; break;
                case 3: _product.thisGpu = db.gpus.SingleOrDefault(x => x.productId == id1); viewModel.Partial = "~/Views/Home/PartialViews/_comparisonGPU.cshtml"; break;
                case 4: _product.thisPsu = db.psus.SingleOrDefault(x => x.productId == id1); viewModel.Partial = "~/Views/Home/PartialViews/_comparisonPSU.cshtml"; break;
                case 5: _product.thisRam = db.rams.SingleOrDefault(x => x.productId == id1); viewModel.Partial = "~/Views/Home/PartialViews/_comparisonRAM.cshtml"; break;
                case 6: _product.thisHardDisc = db.hardDiscs.SingleOrDefault(x => x.productId == id1); viewModel.Partial = "~/Views/Home/PartialViews/_comparisonHardDisc.cshtml"; break;
                case 8: _product.thisMonitor = db.monitors.SingleOrDefault(x => x.productId == id1); viewModel.Partial = "~/Views/Home/PartialViews/_comparisonMonitor.cshtml";  break;
                case 9: _product.thisBox = db.boxes.SingleOrDefault(x => x.productId == id1); viewModel.Partial = "~/Views/Home/PartialViews/_comparisonBox.cshtml"; break;

            }
            viewModel.product1 = _product;
           
            if (id2!=0)
            {
                var _product2 = db.products.SingleOrDefault(x => x.id == id2);
                switch (_product.categoryId)
                {
                    case 1: _product2.thisMotherboard = db.motherboards.SingleOrDefault(x => x.productId == id2); break;
                    case 2: _product2.thisCpu = db.cpus.SingleOrDefault(x => x.productId == id2); break;
                    case 3: _product2.thisGpu = db.gpus.SingleOrDefault(x => x.productId == id2); break;
                    case 4: _product2.thisPsu = db.psus.SingleOrDefault(x => x.productId == id2); break;
                    case 5: _product2.thisRam = db.rams.SingleOrDefault(x => x.productId == id2); break;
                    case 6: _product2.thisHardDisc = db.hardDiscs.SingleOrDefault(x => x.productId == id2); break;
                    case 8: _product2.thisMonitor = db.monitors.SingleOrDefault(x => x.productId == id2); break;
                    case 9: _product2.thisBox = db.boxes.SingleOrDefault(x => x.productId == id2); break;

                }
                viewModel.product2 = _product2;

            }
            if (id3 != 0)
            {
                var _product3 = db.products.SingleOrDefault(x => x.id == id3);
                switch (_product.categoryId)
                {
                    case 1: _product3.thisMotherboard = db.motherboards.SingleOrDefault(x => x.productId == id3); break;
                    case 2: _product3.thisCpu = db.cpus.SingleOrDefault(x => x.productId == id3); break;
                    case 3: _product3.thisGpu = db.gpus.SingleOrDefault(x => x.productId == id3); break;
                    case 4: _product3.thisPsu = db.psus.SingleOrDefault(x => x.productId == id3); break;
                    case 5: _product3.thisRam = db.rams.SingleOrDefault(x => x.productId == id3); break;
                    case 6: _product3.thisHardDisc = db.hardDiscs.SingleOrDefault(x => x.productId == id3); break;
                    case 8: _product3.thisMonitor = db.monitors.SingleOrDefault(x => x.productId == id3); break;
                    case 9: _product3.thisBox = db.boxes.SingleOrDefault(x => x.productId == id3); break;

                }
                viewModel.product3 = _product3;

            }
            if (id4 != 0)
            {
                var _product4 = db.products.SingleOrDefault(x => x.id == id4);
                switch (_product.categoryId)
                {
                    case 1: _product4.thisMotherboard = db.motherboards.SingleOrDefault(x => x.productId == id4); break;
                    case 2: _product4.thisCpu = db.cpus.SingleOrDefault(x => x.productId == id4); break;
                    case 3: _product4.thisGpu = db.gpus.SingleOrDefault(x => x.productId == id4); break;
                    case 4: _product4.thisPsu = db.psus.SingleOrDefault(x => x.productId == id4); break;
                    case 5: _product4.thisRam = db.rams.SingleOrDefault(x => x.productId == id4); break;
                    case 6: _product4.thisHardDisc = db.hardDiscs.SingleOrDefault(x => x.productId == id4); break;
                    case 8: _product4.thisMonitor = db.monitors.SingleOrDefault(x => x.productId == id4); break;
                    case 9: _product4.thisBox = db.boxes.SingleOrDefault(x => x.productId == id4); break;

                }
                viewModel.product4 = _product4;

            }
            var thisUser = Me();
            if(this.User.Identity.IsAuthenticated)
            {
                TempData["productsInCart"] = db.carts.Where(x => x.userId == thisUser.UserId).Count();
            }
           
            return View(viewModel);
        }  // Adds Products to the table


        public List<product> GetMyList(int? id) // giving back the correct category list (takes as parameter one categoryID
        {
            List<product> list = new List<product>();
            switch (id)
            {

                case 1: list = db.products.Where(x => x.categoryId == 1).ToList(); break;
                case 2: list = db.products.Where(x => x.categoryId == 2).ToList(); break;
                case 3: list = db.products.Where(x => x.categoryId == 3).ToList(); break;
                case 4: list = db.products.Where(x => x.categoryId == 4).ToList(); break;
                case 5: list = db.products.Where(x => x.categoryId == 5).ToList(); break;
                case 6: list = db.products.Where(x => x.categoryId == 6).ToList(); break;
                case 8: list = db.products.Where(x => x.categoryId == 8).ToList(); break;
                case 9: list = db.products.Where(x => x.categoryId == 9).ToList(); break;
            }
            return list;
        }
             

        public User Me()
        {
            var _thisUser = db.Users.SingleOrDefault(x => x.Email == this.User.Identity.Name);
            return _thisUser;
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
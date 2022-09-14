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

namespace Eshop.Controllers
{
  
    public class HomeController : Controller
    {
        static int _productId = 0;
        static int _desktopId = 0;
        static decimal _price = 0;
        static User ThisUser;
        public Dictionary<string, string> emails;

        private readonly Entities db = new Entities(); //Readonly allagh JJJ
        public Connection connection;
        public HomeController()
        {
            db = new Entities();    //Constructor 
            connection = new Connection();
            emails = new Dictionary<string, string>();
            emails.Add("Yannis", "hxo999@yahoo.gr");
            emails.Add("Thodoris", "thestas@yahoo.com");
            emails.Add("Aris", "zeromixer2010@yahoo.com");
            emails.Add("Alex", "alexmantzaris.kar@gmail.com");
            emails.Add("Tasos", "tasosadam1991@gmail.com");
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



        [Route("/Home/AdminSupport/{receiver}")] //id equals Email here
        public ActionResult AdminSupport(string receiver = "")
        {

            int UserId = 0;
            int AdminId = 0;
            int UserIdjoiningSup = 0;
            ConnectionViewModel viewModel = null;


            ViewBag.NoConnection = "pending";

            if (!db.Users.Any(u => u.Email.Equals(receiver)))
                return Redirect("~/");

            //Getting USER from DB
            var userLoggedIn = db.Users.FirstOrDefault(u => u.Email.Equals(User.Identity.Name));
            var userJoiningSupport = db.Users.FirstOrDefault(u => u.Email.Equals(receiver));
            if (userLoggedIn.orders != null || User.IsInRole("Admin"))
            {

                UserDTO userDTOLoggedIn = new UserDTO();
                var dtoUser = Mapper.Map(userLoggedIn, userDTOLoggedIn);
                if (dtoUser != null)
                {
                    UserId = dtoUser.UserId;
                }
                UserDTO userDToJoiningSupport = new UserDTO();
                var dtoUserJoiningSupport = Mapper.Map(userJoiningSupport, userDToJoiningSupport);

                UserIdjoiningSup = dtoUserJoiningSupport.UserId;

                //Viewbag the bag out of em
                ViewBag.FullName = dtoUser.FirstName + " " + dtoUser.LastName;
                ViewBag.FullNameSupport = dtoUserJoiningSupport.FirstName + " " + dtoUserJoiningSupport.LastName;
                ViewBag.Username = dtoUserJoiningSupport.Username;


                //TempData["User"] = dtoUserJoiningSupport.Username;


                string userType = "admin"; //guest
                if (User.IsInRole("Customer"))
                    userType = "customer"; //owner



                //IF ADMIN IS CONNECTED CHECK IF THE CONNECTION USER-ADMIN DONE
                if (userType == "admin")
                {
                    ViewBag.UserType = "admin";

                    var userAdmin = db.Users.FirstOrDefault(u => u.Email.Equals(receiver)); //FOR TESTING PURPOSES ADD HERE THE EMAIL - FOR SECURITY PURPOSES HERE ADD IDENTITY NAME

                    UserDTO userDTOAdmin = new UserDTO();
                    var dtoAdmin = Mapper.Map(userAdmin, userDTOAdmin);

                    AdminId = userDTOAdmin.UserId;


                    connection = db.Connections.Where(con => con.AdminId == AdminId).FirstOrDefault(); // TO GET FROM CONNECTION ! - CHECK IF ALSO USERID IN CONNECTIONS TO ADD LATER
                    if (connection == null)
                    {
                        ViewBag.NoConnection = "True";
                    }

                    else if (connection.UserId != null)
                    {
                        ViewBag.NoConnection = "Pending";
                    }
                    else if (connection != null)
                    {
                        ViewBag.NoConnection = "Pending";
                    }

                }
                else
                {

                    ViewBag.NoConnection = "False";
                    ViewBag.UserType = userType;

                }

            }
            else
            {
                return Content("<script language='javascript' type='text/javascript'>alert('You need some orders first to contact our Support. Thank you very much!');</script>");
            }


            var userId = 0;
            User user = null;

            if (User.Identity.Name != "")
            {
                user = db.Users.SingleOrDefault(u => u.Email == User.Identity.Name);
                userId = db.Users.SingleOrDefault(u => u.Email == User.Identity.Name).UserId;
                TempData["userId"] = userId;
            }

            var favouriteProducts = db.favourites
              .Where(u => u.userId == UserId)
                .ToList()
               .ToLookup(u => u.ProductId);

            var addedCartProducts = db.carts
             .Where(u => u.userId == UserId)
             .ToList()
             .ToLookup(u => u.productId);

            ViewBag.addedCartProducts = addedCartProducts;
            ViewBag.Favourites = favouriteProducts;


            //ViewBAG MESSAGE COUNT
            var messageCount = db.SupportMessages.Count(x => x.To == UserId && x.Read == false);
            ViewBag.MsgCount = messageCount;
            //RETURN
            TempData["productsInCart"] = db.carts.Where(u => u.userId == userId).Count();


            if (User.IsInRole("Customer"))
            {
                viewModel = new ConnectionViewModel()
                {
                    IsActive = false,
                    Connection = connection,
                    User = db.Users.SingleOrDefault(u => u.UserId == userJoiningSupport.UserId),
                    Products = db.products.ToList().Take(10),
                    Favourites = favouriteProducts,
                    SupportMessages = db.SupportMessages.ToList()
      


                };
            }
            if (connection != null && User.IsInRole("Admin"))
            {
                viewModel = new ConnectionViewModel()
                {
                    IsActive = true,
                    Admin = db.Users.SingleOrDefault(u => u.UserId == AdminId),
                    Connection = connection,
                    User = db.Users.SingleOrDefault(u => u.UserId == userLoggedIn.UserId),
                    Products = db.products.ToList().Take(10),
                    Favourites = favouriteProducts,
                    SupportMessages = db.SupportMessages.ToList()

                };
            }


            if (ModelState.IsValid)
                return View("AdminSupport", viewModel);


            return View("AdminSupport");
        }
        [HttpPost]
       //id equals Email here
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

            if(conn != null)
            {
                if (User.Identity.Name.ToString() != conn.Email.ToString())
                {
                   
                    TempData["msg"] = "Provide the email you are logged in with. Thank you!";
                    return View("AdminSupport", conn);
                }

                userInDb = db.Users.FirstOrDefault(u => u.Email == conn.Email);

                mail.From = new MailAddress("zeromixer2000@gmail.com");
                //add user name to some one
                mail.Subject = $"{conn.Email} contacted our support. Answer need to be made before the passage of 24 hours";
                mail.Body = conn.Email +" "+ "Message: "+" " +conn.Inquiry;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Credentials = new System.Net.NetworkCredential
                         ("zeromixer2000@gmail.com", "xiezsjucrmgpoxug");
                smtp.EnableSsl = true;
                smtp.Port = 587;

                smtp.Send(mail);
                //TempData["msg"] = "Email Sent";


                var contact = new Contactform()
                {
                    FirstName = conn.FirstName,
                    LastName = conn.LastName,
                    Email = conn.Email,
                    Enquiry = conn.Inquiry
                };
                string userFirstName = userInDb.FirstName;
                string userLastName = userInDb.LastName;
                   
                if (string.IsNullOrEmpty(userFirstName))
                {
                    userInDb.FirstName = conn.FirstName;
                }
                if (string.IsNullOrEmpty(userLastName))
                {
                    userInDb.LastName = conn.LastName;
                }
                db.Contactforms.Add(contact);
                db.SaveChanges();
                TempData["emailSentNotification"] = "Message Sent! Thank you very much. Contact support service will answer you as soon as possible.";
            }
           

            return RedirectToAction("AdminSupport");
        }


        [HttpPost]
        [Route("Home/SendMessage?friend={friend}&{message}")] //id equals Email here
        public ActionResult SendMessage(string friend, string message)
        {
            var LoggedUserEmail = User.Identity.Name;
            var dbLoggedUser = db.Users.FirstOrDefault(u => u.Email == LoggedUserEmail);
            //GETING FIRST USER

            var userId = dbLoggedUser.UserId;
            //GETING SECOND
            var NextUser = db.Users.SingleOrDefault(u => u.Email == friend);


            var userId2 = NextUser.UserId;
            //SAVE MESSAGES

            SupportMessage support = new SupportMessage();

            support.From = userId;
            support.To = userId2;
            support.Message = message;
            support.DateSent = DateTime.Now;
            support.Read = false;
            support.FirstName = dbLoggedUser.FirstName;
            support.LastName = dbLoggedUser.LastName;
            support.FromId = userId;
            support.UserName = dbLoggedUser.Username;
            support.UserId = dbLoggedUser.UserId;

            db.SupportMessages.AddOrUpdate(support);
            db.SaveChanges();
            TempData["message"] = message;
            ConnectionViewModel viewModel = new ConnectionViewModel()
            {
                From = userId,
                To = userId2,
                Message = message,
                DateSent = DateTime.Now,
                Read = false,
                FirstName = dbLoggedUser.FirstName,
                LastName = dbLoggedUser.LastName,
                FromId = userId,
                UserName = dbLoggedUser.Username,
                UserId = dbLoggedUser.UserId
            };

            TempData["message"] = message;
            if (message != null)
                return PartialView("Message", viewModel);


            return PartialView("Message");

        }


        [Route("/Home/SendEmailToAdmin")] //id equals Email here
        //[NonAction]
        public JsonResult SendEmailToAdmin()
        {


            //GET THE CURREMT LOGED IN USER
            string id = null; string receiver = null; string receiverName = null; string verifyUrl = null; string link = string.Empty;
            User user = null; User admin = null;

            //GETING SOME ADMIN
            Random random = new Random();

            if (!User.IsInRole("Admin"))
            {
                id = User.Identity.Name;
                //receiver = emails.ElementAt(random.Next(0, emails.Count)).Value; // COMMENT OUT WHEN DONE CHANGED VALUE FOR TESTING
                //receiverName = emails.FirstOrDefault(u => u.Value == receiver).Key; // COMMENT OUT WHEN DONE CHANGED VALUE FOR TESTING
                receiver = "alexmantzaris.kar@gmail.com";
                receiverName = "z4zAdmin";

                verifyUrl = "/Home/AdminSupport?receiver=" + receiver; //HERE TO SEND ADMIN 

                link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
                user = db.Users.FirstOrDefault(u => u.Email == id);

                TempData["receiver"] = receiver;
            }
            else
            {
                id = User.Identity.Name;
                receiver = db.Users.SingleOrDefault(u => u.Email == id).Email.ToString();
            }



            admin = db.Users.FirstOrDefault(u => u.Email == receiver);
            //CREATING CONNECTION FOR CHAT SUPPORT

            var connection = new Connection();
            connection.Active = false;


            if (!string.IsNullOrEmpty(id) && db.Users.Any(u => u.Email.ToString() == receiver) && user != null && admin != null)
            {
                connection.UserId = user.UserId;
                connection.AdminId = admin.UserId;
                connection.Active = true; //HERE TO CHANGE TO FALSE AND CHANGE IT WHEN THE CONNECTION IS ACTUALLY MADE

            }
            db.Connections.AddOrUpdate(connection);
            db.SaveChanges();



            //Sending the email to some admin
            if (ModelState.IsValid && User.IsInRole("Customer"))
            {
                var senderEmail = new MailAddress("zeromixer2000@gmail.com", "CyberStore");
                var receiverEmail = new MailAddress("alexmantzaris.kar@gmail.com", "Receiver");  //ADD receiver  HERE - FIRST VARIABLE!
                var password = "xiezsjucrmgpoxug";
                var subject = "Help is needed";
                var body = $"Hey , {receiverName} please join the support chat {link}";
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
                return Json(new { status = "Thank you very much admin for showing up. Don't forget to send us the email of your feedback on your way out" }, JsonRequestBehavior.AllowGet);
            }


            return Json(new { status = "Something went wrong, please try again" }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult DisplayUnreadMessages()
        {

            // Get user id
            User user = db.Users.Where(x => x.Email.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = user.UserId;

            // Create a list of unread messages
            List<SupportMessage> list = db.SupportMessages.Where(x => x.To == userId && x.Read == false).ToArray().Select(x => new SupportMessage(x.ToString())).ToList();

            // Make unread read
            db.SupportMessages.Where(x => x.To == userId && x.Read == false).ToList().ForEach(x => x.Read = true);
            db.SaveChanges();

            // Return json
            return Json(list);
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
                Monitor = monitor

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
            var desktopPrice = thisUser.Desktop.price;
            var _totalPrice = totalProducts + desktopPrice;

            var viewModel = new OrderViewModel()
            {
                CartItems = cart.ToList(),
                TotalPrice = (int)_totalPrice,
                UserId = userId,
                CreatedAt = DateTime.Now,
                TrackingNumber = RandomString(14)

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
                
                var _totalPrice = totalProducts + desktopPrice;
                
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
                   
                    thisUser.Desktop.product = _nullProduct;
                    thisUser.Desktop.product1 = _nullProduct;
                    thisUser.Desktop.product2 = _nullProduct;
                    thisUser.Desktop.product3 = _nullProduct;
                    thisUser.Desktop.product4 = _nullProduct;
                    thisUser.Desktop.product5 = _nullProduct;

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
                    
                    db.products.SingleOrDefault(p => p.id == cartItem.productId).stock -= cartItem.quantity;
                    db.products.SingleOrDefault(p => p.id == cartItem.productId).sales += cartItem.quantity;
                    db.OrderItems.Add(OrderItem);
                    db.carts.Remove(cartItem);
                    db.SaveChanges();
                }                                                                                  
            }
   

            TempData["OrderComplete"] = "Your purchace was successful. Thank you!";
           
            return RedirectToAction("Checkout", "Home");
        }
        [HttpPost]
        public void SendToDesktop(int id)
        {
            var thisUser = Me();
            var product = db.products.SingleOrDefault(x => x.id == id);
            var desktop = thisUser.Desktop;

            switch (product.categoryId)
            {
                case 1: desktop.MOTHERBOARD = product.id; desktop.product1 = product; break;
                case 2: desktop.CPU = product.id; desktop.product = product; break;
                case 3: desktop.GPU = product.id; desktop.product2 = product; break;
                case 4: desktop.PSU = product.id; desktop.product4 = product; break;
                case 5: desktop.RAM = product.id; desktop.product5 = product; break;
                case 6: desktop.HARDDISC = product.id; desktop.product3 = product; break;
            }

            db.Desktops.AddOrUpdate(desktop);
            db.SaveChanges();
        }
        public ActionResult YourDesktop()
        {
             
            var thisUser = Me();
            double TotalPrice = 0;
            Desktop thisDesktop = thisUser.Desktop;                             
            product _nullProduct = db.products.SingleOrDefault(x => x.categoryId == 7);      
            thisDesktop.price = Convert.ToDecimal(TotalPrice);

            if (thisDesktop.product == null) thisDesktop.product = _nullProduct; else thisDesktop.price += thisDesktop.product.price;
            if (thisDesktop.product1 == null) thisDesktop.product1 = _nullProduct; else thisDesktop.price += thisDesktop.product1.price;
            if (thisDesktop.product2 == null) thisDesktop.product2 = _nullProduct; else thisDesktop.price += thisDesktop.product2.price;
            if (thisDesktop.product3 == null) thisDesktop.product3 = _nullProduct; else thisDesktop.price += thisDesktop.product3.price;
            if (thisDesktop.product4 == null) thisDesktop.product4 = _nullProduct; else thisDesktop.price += thisDesktop.product4.price;
            if (thisDesktop.product5 == null) thisDesktop.product5 = _nullProduct; else thisDesktop.price += thisDesktop.product5.price;


            db.Desktops.AddOrUpdate(thisDesktop);
            db.SaveChanges();

            DesktopViewModel deskViewModel = new DesktopViewModel()
            {
                desktop = thisDesktop,

                ProdMotherboards = db.products.Where(x => x.categoryId == 1 &&  x.stock>0),
                ProdCpus = db.products.Where(x => x.categoryId == 2 && x.stock > 0),
                ProdGpus = db.products.Where(x => x.categoryId == 3 && x.stock > 0),
                ProdPsus = db.products.Where(x => x.categoryId == 4 && x.stock > 0),
                ProdRams = db.products.Where(x => x.categoryId == 5 && x.stock > 0),
                ProdHardDiscs = db.products.Where(x => x.categoryId == 6 && x.stock > 0),

                Cpus = db.cpus,
                Motherboards = db.motherboards,

            };

            return View(deskViewModel);
        }
        public ActionResult ProductComparsion(int id, int id2 = 0)
        {
            ComparsionViewModel Model = new ComparsionViewModel();

            Model.SelectedProduct = db.products.SingleOrDefault(x => x.id == id);
            if (id2 == 0)
            {
                switch (Model.SelectedProduct.categoryId)
                {

                    case 1: Model.SelectedList = db.products.Where(x => x.categoryId == 1 && x.id != Model.SelectedProduct.id); break;
                    case 2: Model.SelectedList = db.products.Where(x => x.categoryId == 2 && x.id != Model.SelectedProduct.id); break;
                    case 3: Model.SelectedList = db.products.Where(x => x.categoryId == 3 && x.id != Model.SelectedProduct.id); break;
                    case 4: Model.SelectedList = db.products.Where(x => x.categoryId == 4 && x.id != Model.SelectedProduct.id); break;
                    case 5: Model.SelectedList = db.products.Where(x => x.categoryId == 5 && x.id != Model.SelectedProduct.id); break;
                    case 6: Model.SelectedList = db.products.Where(x => x.categoryId == 6 && x.id != Model.SelectedProduct.id); break;
                    case 8: Model.SelectedList = db.products.Where(x => x.categoryId == 8 && x.id != Model.SelectedProduct.id); break;



                }
                Model.TwoProducts = false;
                return View(Model);

            }
            else
            {

                Model.ProductToCompare = db.products.SingleOrDefault(x => x.id == id2);

                switch (Model.SelectedProduct.categoryId)
                {

                    case 1: Model.Motherboard1 = db.motherboards.SingleOrDefault(x => x.productId == Model.SelectedProduct.id); Model.Motherboard2 = db.motherboards.SingleOrDefault(x => x.productId == Model.ProductToCompare.id); break;
                    case 2: Model.cpu1 = db.cpus.SingleOrDefault(x => x.productId == Model.SelectedProduct.id); Model.cpu2 = db.cpus.SingleOrDefault(x => x.productId == Model.ProductToCompare.id); break;
                    case 3: Model.gpu1 = db.gpus.SingleOrDefault(x => x.productId == Model.SelectedProduct.id); Model.gpu2 = db.gpus.SingleOrDefault(x => x.productId == Model.ProductToCompare.id); break;
                    case 4: Model.psu1 = db.psus.SingleOrDefault(x => x.productId == Model.SelectedProduct.id); Model.psu2 = db.psus.SingleOrDefault(x => x.productId == Model.ProductToCompare.id); break;
                    case 5: Model.ram1 = db.rams.SingleOrDefault(x => x.productId == Model.SelectedProduct.id); Model.ram2 = db.rams.SingleOrDefault(x => x.productId == Model.ProductToCompare.id); break;
                    case 6: Model.hardDisc1 = db.hardDiscs.SingleOrDefault(x => x.productId == Model.SelectedProduct.id); Model.hardDisc2 = db.hardDiscs.SingleOrDefault(x => x.productId == Model.ProductToCompare.id); break;
                    case 8: Model.monitor1 = db.monitors.SingleOrDefault(x => x.productId == Model.SelectedProduct.id); Model.monitor2 = db.monitors.SingleOrDefault(x => x.productId == Model.ProductToCompare.id); break;
                }
                Model.TwoProducts = true;
                return View(Model);

            }

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
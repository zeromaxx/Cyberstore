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

        public ActionResult AboutUs()
        {

            return View();
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
            { return View("AdminSupport", viewModel); }

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


        [Route("/Home/SendEmailToAdmin")]
        public JsonResult SendEmailToAdmin()
        {
            //GET THE CURREMT LOGED IN USER
            string id = null; string receiver = null; string receiverName = null; string verifyUrl = null; string link = string.Empty;
            User user = null; User admin = null; bool IsTheSameConnection = true; var adminId = 0;
            //GETING SOME ADMIN
            Random random = new Random();
            if (!User.IsInRole("Admin"))
            {
                id = User.Identity.Name;
                //receiver = emails.ElementAt(random.Next(0, emails.Count)).Value; // COMMENT OUT WHEN DONE CHANGED VALUE FOR TESTING
                //receiverName = emails.FirstOrDefault(u => u.Value == receiver).Key; // COMMENT OUT WHEN DONE CHANGED VALUE FOR TESTING
                receiver = "alexmantzaris.kar@gmail.com";
                receiverName = "alex";
                verifyUrl = "/Home/AdminSupport?receiver=" + receiver; //HERE TO SEND ADMIN 
                link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
                user = db.Users.FirstOrDefault(u => u.Email == id);
                TempData["receiver"] = receiver;
            }
            else
            {
                id = User.Identity.Name;
                receiver = db.Users.SingleOrDefault(u => u.Email == id).Email.ToString();
                adminId = db.Users.SingleOrDefault(u => u.Email == id).UserId;
            }
            admin = db.Users.FirstOrDefault(u => u.Email == receiver);
            //CREATING CONNECTION FOR CHAT SUPPORT
            var connection = new Connection();
            var newConnection = new Connection();
            connection.Active = false;
            if (!string.IsNullOrEmpty(id) && db.Users.Any(u => u.Email.ToString() == receiver) && user != null && admin != null)
            {
                //GET ALL CONNECTIONS
                var connections = db.Connections.ToList();
                //CHECK IF A CONNECTION HAS THE ADMINID
                if (connections.Count == 0)
                {
                    connection.UserId = user.UserId;
                    connection.AdminId = admin.UserId;
                    connection.Active = true;
                    connection.Status = "Live";
                    db.Connections.AddOrUpdate(connection);
                    db.SaveChanges();
                }
                else
                {
                    foreach (var con in connections)
                    {
                        //CREATE A NEW CONNECTION
                        //if (con.AdminId == admin.UserId)
                        //{
                        //    connection = con;
                        //}
                        if (con.AdminId == admin.UserId && con.UserId != user.UserId)
                        {
                            IsTheSameConnection = false;
                        }
                        if (con.AdminId == admin.UserId && con.UserId == user.UserId)
                        {
                            IsTheSameConnection = true;
                        }
                        //if (con.AdminId == adminId && con.UserId == adminId)
                        //{
                        //    IsTheSameConnection = true;
                        //}
                    }
                    if (!IsTheSameConnection)
                    {
                        //if (db.Connections.ToList().Contains(connection))
                        //{
                        newConnection.Status = "Pending";
                        newConnection.Active = true;
                        newConnection.AdminId = admin.UserId;
                        newConnection.UserId = user.UserId;
                        db.Connections.AddOrUpdate(newConnection);
                        db.SaveChanges();
                        //}
                    }
                    else
                    {
                        connection.UserId = user.UserId;
                        connection.AdminId = admin.UserId;
                        connection.Active = true;
                        connection.Status = "Live";
                        db.Connections.AddOrUpdate(connection);
                        db.SaveChanges();
                    }
                }
            }
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
   

            TempData["OrderComplete"] = "Your purchace was successful. Thank you!";
           
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
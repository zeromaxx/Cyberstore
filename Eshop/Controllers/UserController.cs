using AutoMapper;
using Eshop;
using Eshop.Models;
using Eshop.Models.Dtos;
using Eshop.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
namespace Eshop.Controllers
{
    public class UserController : Controller
    {
        //private readonly UserRoleProvider userRole;
        private readonly Entities db;
        public UserController()
        {
            //userRole = new UserRoleProvider();
            db = new Entities();
        }



        //REGISTERED USERS
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }
        public ActionResult Register()
        {
            //Confirm user is not logged in
            string username = User.Identity.Name;
            var user = db.Users.SingleOrDefault(u => u.Email == username);

            if (!string.IsNullOrEmpty(username))
            {
                return Content("<script language='javascript' type='text/javascript'>alert('You are allready logged in!');</script>");
            }

            var viewModel = new RegisterView();
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Exclude = "IsEmailVerified,ActivationCode")] RegisterView reg)
        {
            reg.EmailExists = false;
            if (!ModelState.IsValid)
            {
                return View("Register", reg);
            }
            bool Status = false;
            string message = "";
            var _nullProduct = db.products.SingleOrDefault(x => x.categoryId == 7);
            ViewBag.Message = message;
            try
            {
                var regModel = new User()
                {
                    Username = reg.Username,
                    Password = GetMD5HashData(reg.Password),
                    Confirmpwd = GetMD5HashData(reg.Confirmpwd),
                    Email = reg.Email,
                    Desktop = new Desktop()
                    {
                        Name = "MyCustomDesktop",
                        product = _nullProduct,
                        product1 = _nullProduct,
                        product2 = _nullProduct,
                        product3 = _nullProduct,
                        product4 = _nullProduct,
                        product5 = _nullProduct,
                        product6=  _nullProduct
                    }
                };
                regModel.Desktop.price = 0;
                //CHECKING IF EMAIL EXISTS
                if (ModelState.IsValid)
                {
                    //MAKING SURE USERNAME IS UNIQUE AND EMAIL AS WELL
                    var doesExist = DoesEmailExist(reg.Email);
                    if (doesExist)
                    {
                        ModelState.AddModelError("EmailExist", "Email already exists");
                        reg.EmailExists = true;
                        return View(reg);
                    }
                    if (db.Users.Any(user => user.Username.Equals(reg.Username)))
                    {
                        ModelState.AddModelError("", "Username " + reg.Username + "is taken.");
                        reg.Username = "";
                        return View(reg);
                    }
                    #region Generate Activation Code
                    regModel.ActivationCode = Guid.NewGuid();
                    #endregion
                    regModel.IsEmailVerified = false;
                    #region Save to Database
                    db.Users.Add(regModel);
                    db.SaveChanges();
                    SendVerificationLinkEmail(regModel.Email, regModel.ActivationCode.ToString());
                    message = "Registration successfull!";
                    Status = true;
                    #endregion
                }
                else
                {
                    message = "Invalid Request";
                }
                ViewBag.Message = message;
                ViewBag.Status = Status;
                #region start DEFAULT CUSTOMER ROLE
                //ADDING DEFAULT CUSTOMER ROLE TO LOGGED IN USER
                var currentUser = db.Users.First(u => u.Username == regModel.Username);
                string role = "Customer";
                //FOR REFACTORING ADD EMAILS TO A LIST AND USE IF CONTAINS
                if (reg.Email == "hxo999@yahoo.gr" || reg.Email == "thestas@yahoo.com" || reg.Email == "zero12@yahoo.com" || reg.Email == "alexmantzaris.kar@gmail.com" || reg.Email == "tasosadam1991@gmail.com")
                {
                    role = "Admin";
                }
                if (role == "Admin")
                {
                    db.UserRoleMappings.Add(new UserRoleMapping()
                    {
                        UserId = currentUser.UserId,
                        RoleId = 1
                    });
                }
                else
                {
                    db.UserRoleMappings.Add(new UserRoleMapping()
                    {
                        UserId = currentUser.UserId,
                        RoleId = 2
                    });
                }
                db.SaveChanges();
                #endregion
                return View(reg);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        [HttpPost]
        [Route("User/EmailNotSent/{id?}")]

        public ActionResult EmailNotSent(string id) //ID EQUALS EMAIL
        {
            User userInDb = null;
           
            if (id == null)
                return RedirectToAction("Login");
            userInDb = db.Users.SingleOrDefault(u => u.Email == id);
            
            if (userInDb == null)
                return RedirectToAction("Login");

            var code = userInDb.ActivationCode.ToString();
            SendVerificationLinkEmail(id, code);

            return RedirectToAction("Login");
        }
        public ActionResult Login()
        {
            var _Users = db.Users.ToList();
            var viewmodel = new RegistrationViewModel()
            {
                User = new User(),
                Users = _Users
            };
            return View(viewmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Exclude = "User,Email,Users")] RegistrationViewModel reg, string ReturnUrl = "")
        {
            string message = "";
            var passToCheck = GetMD5HashData(reg.Password);


            try
            {
                var user = db.Users.SingleOrDefault(v => v.Username == reg.Username);

              
                if (user != null)
                {

                    var email = user.Email;
                    var userByEmail = db.Users.Where(a => a.Email == email).FirstOrDefault();
                    if (userByEmail != null)
                    {
                        if (userByEmail.IsEmailVerified != true)
                        {
                            TempData["Unverified Email"] = "Please verify your email first";

                        }
                        else
                        {
                            if (user != null)
                            {
                                if (passToCheck == user.Password)
                                {

                                    //ADDING COOKIE
                                    var userRoleMapping = db.UserRoleMappings.Single(u => u.UserId == user.UserId);
                                    var role = db.Roles.Where(p => p.Id == userRoleMapping.RoleId).Single();
                                    var roles = role.RoleName;
                                    int timeout = reg.RememberMe ? 525600 : 200; //525600 min = 1 year
                                    var ticket = new FormsAuthenticationTicket(1,
                                                     email,
                                                     DateTime.Now,
                                                     DateTime.Now.AddMinutes(timeout),
                                                     reg.RememberMe, roles);
                                    string encrypted = FormsAuthentication.Encrypt(ticket);
                                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                                    //cookie.Expires = DateTime.Now.AddMinutes(timeout);
                                    cookie.HttpOnly = true;
                                    Response.Cookies.Add(cookie);

                                    var details = (from userlist in db.Users
                                                   where userlist.Username == reg.Username && userlist.Password == passToCheck
                                                   select new
                                                   {
                                                       userlist.UserId,
                                                       userlist.Username
                                                   }).ToList();
                                    if (details.FirstOrDefault() != null)
                                    {
                                        Session["UserId"] = details.FirstOrDefault().UserId;
                                        Session["Username"] = details.FirstOrDefault().Username;
                                        //return RedirectToAction("Index", new RouteValueDictionary(
                                        //         new { controller = "Home", action = "Index", id = reg.Username }));
                                        return RedirectToAction("Index", "Home");
                                    }
                                    if (user == null)
                                    {
                                        ModelState.AddModelError("", "Invalid Credentials");
                                    }
                                    if (Url.IsLocalUrl(ReturnUrl))
                                    {
                                        return Redirect(ReturnUrl);
                                    }
                                    else
                                    {
                                        //{
                                        //    return RedirectToAction("Index", new RouteValueDictionary(
                                        //             new { controller = "Home", action = "Index", id = reg.Username }));
                                        return RedirectToAction("Index", "Home");
                                    }
                                }
                                else
                                {
                                    TempData["loginWrongMessage"] = "Invalid credentials provided";
                                }
                            }
                            else
                            {
                                ViewBag.Message = "Invalid credentials provided";
                            }
                            ViewBag.Message = message;
                        }
                    }


                }
                else
                {
                    TempData["loginWrongMessage"] = "Please try again! Wrong user name or password";
                    return View(reg);
                }

              
            }
            catch (DbEntityValidationException e)
            {
               
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            return View(reg);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }
        [NonAction]
        public bool DoesEmailExist(string Email)
        {
            var v = db.Users.Where(a => a.Email == Email).FirstOrDefault();
            return v == null ? false : true;
        }
        [NonAction]
        public void SendVerificationLinkEmail(string email, string activationCode, string emailFor = "Verify") //CAN ALSO INPUT A 3rd VARIABLE FOR FORGOT PASSWORD VERIFICATION AND CHECK IF IS TRUE
        {

            var user = db.Users.SingleOrDefault(u => u.Email == email);
            var verifyUrl = $"/User/{emailFor}/{activationCode}";
            string subject = "";
            string body = "";
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            //Forgot password verification url
            if (emailFor != "Verify")
            {
                verifyUrl = $"/User{emailFor}/{user.ResetPasswordCode}";

                subject = "Reset Password";
                body = "Hi, We got this request for reseting your account password. Please click on the below link to reset your password " +
                    " " + link + " " + "Reset Password link";
            }
            else
            {
                //First url verification of Email
                subject = "Your account is successfully created!";
                body = "We are excited to tell you that your OP Account is " +
                   "successfully created. Please click on the below link to verify your account" +
                   " " + link + " ";
            }
            var fromEmail = new MailAddress("finalproject20001@gmail.com");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "lmnanzdhnmlnzzpl";


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body
                //IsBodyHtml = true
            })
                smtp.Send(message);
        }

        //Forgot Password

        public ActionResult ForgotPassword()
        {
            return View();
        }



        [Route("User/Verify/{id?}")]
        [HttpGet]
        public ActionResult Verify(string id)
        {
            var viewModel = new EmailVerifyViewModel()
            {
                EmailToken = id
            };
            return View("VerifyAccount", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string Email)
        {

            //Verify Email
            TempData["ForgotPassword"] = "";
            bool status = false;


            var account = db.Users.Where(a => a.Email == Email).FirstOrDefault();
            if (account != null)
            {
                //SEND EMAIL FOR RESET PASSWORD
                string resetCode = Guid.NewGuid().ToString();
                SendVerificationLinkEmail(Email, resetCode, "ResetPassword");
                account.ResetPasswordCode = resetCode;

                //DISABLE SECOND PASSWORD MATCHING - BECAUSE WE ADDED CONFIRM PASSWORD PROPERTY
                db.Configuration.ValidateOnSaveEnabled = false;

                db.SaveChanges();

                TempData["ForgotPassword"] = "Reset password link has been sent to " + Email;

            }
            else
            {
                TempData["ForgotPassword"] = "Account not found";
            }

            return View();
        }


        [Route("User/ResetPassword/{id?}")]
        [HttpGet]
        public ActionResult ResetPassword(string id)
        {
            //Verify the reset link
            var user = db.Users.Where(u => u.ResetPasswordCode == id).FirstOrDefault();
            if (user != null)
            {
                ResetPasswordModel model = new ResetPasswordModel();
                model.ResetCode = id;
                return View(model);
            }
            else
            {
                return HttpNotFound();
            }
            //GET ACCOUNT FROM LINK

            //redirect to reset password page
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            TempData["passwordReset"] = "";
            if (ModelState.IsValid)
            {
                var user = db.Users.Where(u => u.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                if (user != null)
                {
                    user.Password = GetMD5HashData(model.NewPassword);
                    user.Confirmpwd = GetMD5HashData(model.NewPassword);
                    user.ResetPasswordCode = "";
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();

                    TempData["passwordReset"] = "New password updated successfully";
                    return RedirectToAction("Login", "User");
                }
            }
            else
            {
                TempData["passwordReset"] = "Something is invalid";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyAccount(EmailVerifyViewModel id)
        {
            var User = db.Users.Where(a => a.ActivationCode.ToString() == id.EmailToken).FirstOrDefault();
            var Code = db.Users.Where(a => a.ActivationCode.ToString() == id.EmailToken).Select(a => a.ActivationCode).FirstOrDefault().ToString();
            if (Code == id.EmailToken)
            {
                if (User.IsEmailVerified == true)
                {
                    return View("EmailIsVerified");
                }
                User.IsEmailVerified = true;
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Home");
        }
       
        private string GetMD5HashData(string data)
        {
            //create new instance of md5
            MD5 md5 = MD5.Create();
            //convert the input text to array of bytes
            byte[] hashData = md5.ComputeHash(Encoding.Default.GetBytes(data));
            //create new instance of StringBuilder to save hashed data
            StringBuilder returnValue = new StringBuilder();
            //loop for each byte and add it to StringBuilder
            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString());
            }
            // return hexadecimal string
            return returnValue.ToString();
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
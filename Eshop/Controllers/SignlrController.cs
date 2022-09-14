using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Eshop.Controllers
{
    public class SignlrController : Controller
    {
        private readonly Entities db;
        public SignlrController()
        {
            //userRole = new UserRoleProvider();
            db = new Entities();
        }


        // GET: Signlr

        [HttpPost]
        public Boolean CheckIfConnected(string username)
        {
            if(username == null)
                throw new HttpException(400, "Bad Request");
            var userEmail = User.Identity.Name;
            var userId = db.Users.FirstOrDefault(u => u.Email.Equals(userEmail)).UserId;


            var friendId = db.Users.FirstOrDefault(u => u.Username == username).UserId;


            var connectionTestA = db.Connections.Any(con => con.UserId == friendId && con.AdminId == userId);
            var connectionTestB = db.Connections.Any(con => con.UserId == userId && con.AdminId == friendId);
            if (connectionTestA == true || connectionTestB == true)
                return true;

            return false;

        }


        [Authorize]
        public ActionResult Test(string username = "")
        {
           
           

            // Check if user exists
            if (!db.Users.Any(x => x.Username.Equals(username)))
            {
                return Redirect("~/");
            }

            // ViewBag username
            ViewBag.Username = username;

            // Get logged in user's username
            string user = User.Identity.Name;

            // Viewbag user's full name
            User userDTO = db.Users.Where(x => x.Username.Equals(user)).FirstOrDefault();
            ViewBag.FullName = userDTO.FirstName + " " + userDTO.LastName;

            // Get user's id
            int userId = userDTO.UserId;

            // ViewBag user id
            ViewBag.UserId = userId;

            // Get viewing full name
            User userDTO2 = db.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            ViewBag.ViewingFullName = userDTO2.FirstName + " " + userDTO2.LastName;

            // Get username's image
            ViewBag.UsernameImage = userDTO2.UserId + ".jpg";

            // Viewbag user type

            string userType = "guest";

            if (username.Equals(user))
                userType = "owner";

            ViewBag.UserType = userType;

            // Check if they are friends

            if (userType == "guest")
            {
                User u1 = db.Users.Where(x => x.Username.Equals(user)).FirstOrDefault();
                int id1 = u1.UserId;

                User u2 = db.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
                int id2 = u2.UserId;

                Connection f1 = db.Connections.Where(x => x.UserId == id1 && x.AdminId == id2).FirstOrDefault();
                Connection f2 = db.Connections.Where(x => x.AdminId == id1 && x.UserId == id2).FirstOrDefault();


                if (f1 == null && f2 == null)
                {
                    ViewBag.NotFriends = "True";
                }

                if (f1 != null)
                {
                    if ((bool)!f1.Active)
                    {
                        ViewBag.NotFriends = "Pending";
                    }
                }

                if (f2 != null)
                {
                    if ((bool)!f2.Active)
                    {
                        ViewBag.NotFriends = "Pending";
                    }
                }

            }

            // Viewbag request count

            var friendCount = db.Connections.Count(x => x.AdminId == userId && x.Active == false);

            if (friendCount > 0)
            {
                ViewBag.FRCount = friendCount;
            }

            // Viewbag friend count

            User uDTO = db.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            int usernameId = uDTO.UserId;

            var friendCount2 = db.Connections.Count(x => x.AdminId == usernameId && x.Active == true || x.UserId == usernameId && x.Active == true);

            ViewBag.FCount = friendCount2;

            // Viewbag message count

            var messageCount = db.SupportMessages.Count(x => x.To == userId && x.Read == false);

            ViewBag.MsgCount = messageCount;

            // Viewbag user wall
         

            // Return
            return View();
        }
        // POST: Signlr/DisplayUnreadMessages
        [HttpPost]
        public JsonResult DisplayUnreadMessages()
        {
            
            // Get user id
            User user = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = user.UserId;

            // Create a list of unread messages
            List<SupportMessage> list = db.SupportMessages.Where(x => x.To == userId && x.Read == false).ToArray().Select(x => new SupportMessage(x.ToString())).ToList();

            // Make unread read
            db.SupportMessages.Where(x => x.To == userId && x.Read == false).ToList().ForEach(x => x.Read = true);
            db.SaveChanges();

            // Return json
            return Json(list);
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
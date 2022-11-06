using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Eshop.Controllers
{
    public class MessagesController : Controller
    {
        private Entities db = new Entities();
        public MessagesController()
        {

        }
        // GET: Messages
        [HttpPost]
        public ActionResult SaveMessage(string value, int userId)
        {

            var userid = db.messages.Where(u => u.userId == userId);

            switch (value)
            {
                case "help": case " bug":
                    var message = new message()
                    {
                        message1 = value,
                        userId = userId,
                        botReply = "Did you find a bug or problem? Please visit our technical support for further assistance"
                    };
                    db.messages.Add(message);
                    db.SaveChanges();
                    //return Json(new { status = message.botReply }, JsonRequestBehavior.AllowGet);
                    return PartialView(message);
                case "buy":
                    return Json(new { status = "Please visit the products page. Our shop offers a wide variety of products you can buy" }, JsonRequestBehavior.AllowGet);
                default:
                    return Json(new { status = "Not exactly sure what you mean.Please be more specific" }, JsonRequestBehavior.AllowGet);
            }           

        }
    }
}
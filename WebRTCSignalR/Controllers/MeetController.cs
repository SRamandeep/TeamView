using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebRTCSignalR.Models;
using Microsoft.AspNet.Identity;

namespace WebRTCSignalR.Controllers
{
    [Authorize]
    public class MeetController : Controller
    {
        // GET: Meet
        public ActionResult Index()
        {
            string userId = HttpContext.User.Identity.GetUserId();

            return View(model: userId);
        }

        public ActionResult Parents()
        {
            ApplicationUser cUser = new ApplicationDbContext().Users.Find(HttpContext.User.Identity.GetUserId());

            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TwitchBotAdmin.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index(int id = 500)
        {
            return View(id.ToString());
        }
    }
}
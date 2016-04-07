using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TwitchBotAdmin.Controllers
{
    public class MyChannelController : Controller
    {
        // GET: MyChannel
        public ActionResult Index()
        {
            //get channel record from db by current user

            //redirect to standard channel controller?
            return View();
        }

        [HttpPost]
        public ActionResult Edit(/*viewmodel*/)
        {
            return View();
        }
    }
}
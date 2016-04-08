using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TwitchBotAdmin.Controllers
{
    [Authorize]
    public class ChannelController : Controller
    {
        [Route("channel/{channelName?}")]
        public ActionResult Index(string channelName)
        {
            //default to your own channel
            if (string.IsNullOrEmpty(channelName))
                channelName = User.Identity.Name;

            //this is the channel owner if channel name and user name are the same
            bool channelOwner = (channelName.ToLowerInvariant() == User.Identity.Name.ToLowerInvariant());
                

            
            return View();
        }


        [HttpPost]
        public ActionResult Edit(/*viewmodel*/)
        {
            return View();
        }
    }
}
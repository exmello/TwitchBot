using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TwitchBot.Data;
using TwitchBotAdmin.Models;

namespace TwitchBotAdmin.Controllers
{
    [Authorize]
    public class ChannelController : Controller
    {
        private IChannelRepository db = new SqlChannelRepository();

        [Route("channel/{channelName?}")]
        public ActionResult Index(string channelName)
        {
            //default to your own channel
            if (string.IsNullOrEmpty(channelName))
                channelName = User.Identity.Name;

            //this is the channel owner if channel name and user name are the same
            bool channelOwner = (channelName.ToLowerInvariant() == User.Identity.Name.ToLowerInvariant());

            var data = db.GetChannelForDashboard(channelName);

            if(data == null)
                return HttpNotFound();

            return View(new ChannelDashboardViewModel { ChannelForDashboardResult = data });
        }


        [HttpPost]
        public ActionResult Edit(/*viewmodel*/)
        {
            return View();
        }
    }
}
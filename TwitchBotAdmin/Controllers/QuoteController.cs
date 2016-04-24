using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TwitchBot.Data;
using TwitchBot.Model;
using TwitchBotAdmin.Models;

namespace TwitchBotAdmin.Controllers
{
    [Authorize]
    public class QuoteController : Controller
    {
        private IQuoteRepository db = new SqlQuoteRepository();

        // GET: Quote
        [Route("quote/{channelName?}/")]
        public ActionResult Index(string channelName)
        {
            var quotes = db.GetAll(channelName);

            return View(quotes);
        }

        // POST: Quote/Create
        [HttpPost]
        public ActionResult Create(QuoteEditViewModel quoteCreate, string channelName)
        {
            try
            {
                Quote quote = new Quote
                {
                    Text = quoteCreate.Text,
                    AttributedTo = quoteCreate.AttributedTo,
                    AttributedDate = quoteCreate.AttributedDate
                };

                db.Admin_AddUpdate(quote, channelName);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // POST: Quote/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, QuoteEditViewModel quoteEdit, string channelName)
        {
            try
            {
                Quote quote = db.GetByID(id, channelName);

                quote.Text = quoteEdit.Text;
                quote.AttributedTo = quoteEdit.AttributedTo;
                quote.AttributedDate = quoteEdit.AttributedDate;

                db.Admin_AddUpdate(quote, channelName);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // POST: Quote/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, string channelName)
        {
            try
            {
                db.Delete(id, channelName);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

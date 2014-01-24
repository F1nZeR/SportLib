using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Controllers.Base;
using WebApp.Data;
using WebApp.Filters;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class ChatController : BaseController
    {
        //
        // GET: /Chat/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public void PostMessage(string name, string message)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(message)) return;

            var model = new ChatMessageModel
            {
                Date = DateTime.Now,
                Name = name,
                Message = message
            };
            DataContext.ChatMessages.Add(model);
            DataContext.SaveChanges();
        }

        [AllowCrossSiteJson]
        public JsonResult GetChatMessages(int count)
        {
            return Json(DataContext.ChatMessages.OrderByDescending(x => x.Id).Take(count), JsonRequestBehavior.AllowGet);
        }

        #region Чатики
        public ActionResult Lee()
        {
            return View();
        }

        public ActionResult Sarychev()
        {
            return View();
        }

        public ActionResult Avramov()
        {
            return View();
        }

        public ActionResult Krakovetsky()
        {
            return View();
        }

        public ActionResult Kostya()
        {
            return View();
        }
        #endregion
    }
}
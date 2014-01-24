using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebApp.Controllers.Base;

namespace WebApp.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index(int? id)
        {
            if (id != null)
            {
                var result = DataContext.Sports.Find(id);
                if (result == null)
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
                }
                ViewBag.Sport = result;
            }
            else
            {
                ViewBag.Sport = DataContext.Sports.FirstOrDefault();
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Тут кароч описание. Всем привет котаны и кошечки в это чаттике!";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [ChildActionOnly]
        public ActionResult Navigation()
        {
            var result = this.DataContext.Sports.ToList();
            return this.PartialView(result);
        }
    }
}
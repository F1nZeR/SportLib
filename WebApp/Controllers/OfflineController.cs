using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Controllers.Base;
using WebApp.Data;
using WebApp.Helpers;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin, Editor")]
    public class OfflineController : BaseController
    {
        public ActionResult ExportTournament(int id)
        {
            var xmlCreator = new XmlCreator(DataContext);
            var data = xmlCreator.GetTournamentXml(id);

            var context = new DataContext();
            var tournament = context.Tournaments.Include("Sport").Single(x => x.Id == id);
            var name = string.Format("{1} ({0}).xml", tournament.Sport.Name, tournament.Name);
            
            Response.AddHeader("Content-Disposition", "attachment; filename=" + name);
            return File(data, "application/xml");
        }

        public ActionResult ImportTournament()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportTournament(HttpPostedFileBase fileUpload)
        {
            if (fileUpload == null) return View("Error");

            var xmlWorker = new XmlCreator(DataContext);
            xmlWorker.ImportTournaments(fileUpload);
            return RedirectToAction("Index", "Home");
        }
	}
}
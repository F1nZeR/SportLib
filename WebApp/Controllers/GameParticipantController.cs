using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Controllers.Base;
using WebApp.Models;
using WebApp.Models.ViewModels;

namespace WebApp.Controllers
{
    public class GameParticipantController : BaseController
    {
        [Authorize(Roles = "Admin, Editor")]
        public ActionResult Edit(int id)
        {
            // можно только игроков редактировать
            var player = DataContext.GameParticipantPlayers.SingleOrDefault(x => x.Id == id);
            if (player == null) return HttpNotFound();
            return View(GameParticipantPlayerEditModel.CreateFromModel(player));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Editor")]
        public ActionResult Edit(GameParticipantPlayerEditModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (var prop in model.GameParticipantPlayerProps)
                {
                    DataContext.Entry(prop).State = EntityState.Modified;
                }
                DataContext.SaveChanges();
                return RedirectToAction("Details", new {id = model.Id});
            }
            return View(model);
        }

        public ActionResult Details(int id)
        {
            // можно только игроков
            var player = DataContext.GameParticipantPlayers.SingleOrDefault(x => x.Id == id);
            if (player == null) return HttpNotFound();
            return View(player);
        }
	}
}
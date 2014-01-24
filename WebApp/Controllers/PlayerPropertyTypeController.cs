using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Controllers.Base;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PlayerPropertyTypeController : BaseController
    {
        public ActionResult Create(int sportId)
        {
            var sport = DataContext.Sports.SingleOrDefault(x => x.Id == sportId);
            if (sport == null) return View("Error");

            return View(new PlayerPropertyType { SportId = sportId });
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Id")] PlayerPropertyType model)
        {
            if (ModelState.IsValid)
            {
                DataContext.PlayerPropertyTypes.Add(model);
                DataContext.SaveChanges();
                return RedirectToAction("Details", "Sport", new { id = model.SportId });
            }
            else
            {
                ModelState.AddModelError("", "Исправьте ошибки");
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var playerAttr = DataContext.PlayerPropertyTypes.SingleOrDefault(x => x.Id == id);
            if (playerAttr == null) return View("Error");

            return View(playerAttr);
        }

        [HttpPost]
        public ActionResult Edit(PlayerPropertyType model)
        {
            if (ModelState.IsValid)
            {
                DataContext.PlayerPropertyTypes.Attach(model);
                DataContext.Entry(model).State = EntityState.Modified;
                DataContext.SaveChanges();
                return RedirectToAction("Details", "Sport", new { id = model.SportId });
            }
            else
            {
                ModelState.AddModelError("", "Исправьте ошибки");
            }

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var playerAttr = DataContext.PlayerPropertyTypes.SingleOrDefault(x => x.Id == id);
            if (playerAttr == null) return View("Error");
            DataContext.PlayerPropertyTypes.Remove(playerAttr);
            DataContext.SaveChanges();
            return RedirectToAction("Details", "Sport", new { id = playerAttr.SportId });
        }
    }
}
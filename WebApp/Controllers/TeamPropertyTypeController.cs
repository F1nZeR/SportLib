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
    public class TeamPropertyTypeController : BaseController
    {
        public ActionResult Create(int sportId)
        {
            var sport = DataContext.Sports.SingleOrDefault(x => x.Id == sportId);
            if (sport == null) return View("Error");

            return View(new TeamPropertyType { SportId = sportId });
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Id")] TeamPropertyType model)
        {
            if (ModelState.IsValid)
            {
                DataContext.TeamPropertyTypes.Add(model);
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
            var teamProp = DataContext.TeamPropertyTypes.SingleOrDefault(x => x.Id == id);
            if (teamProp == null) return View("Error");

            return View(teamProp);
        }

        [HttpPost]
        public ActionResult Edit(TeamPropertyType model)
        {
            if (ModelState.IsValid)
            {
                DataContext.TeamPropertyTypes.Attach(model);
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
            var teamProp = DataContext.TeamPropertyTypes.SingleOrDefault(x => x.Id == id);
            if (teamProp == null) return View("Error");
            DataContext.TeamPropertyTypes.Remove(teamProp);
            DataContext.SaveChanges();
            return RedirectToAction("Details", "Sport", new { id = teamProp.SportId });
        }
    }
}
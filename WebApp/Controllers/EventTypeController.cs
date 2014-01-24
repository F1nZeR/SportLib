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
    public class EventTypeController : BaseController
    {
        public ActionResult Create(int sportId)
        {
            var sport = DataContext.Sports.SingleOrDefault(x => x.Id == sportId);
            if (sport == null) return View("Error");

            return View(new EventType {SportId = sportId, IsSystemEventType = false});
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Id")] EventType model)
        {
            if (ModelState.IsValid)
            {
                DataContext.EventTypes.Add(model);
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
            var eventType = DataContext.EventTypes.SingleOrDefault(x => x.Id == id);
            if (eventType == null) return View("Error");

            return View(eventType);
        }

        [HttpPost]
        public ActionResult Edit(EventType model)
        {
            if (ModelState.IsValid)
            {
                DataContext.EventTypes.Attach(model);
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

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var eventType = DataContext.EventTypes.SingleOrDefault(x => x.Id == id);
            if (eventType == null) return HttpNotFound();
            DataContext.EventTypes.Remove(eventType);
            DataContext.SaveChanges();
            return RedirectToAction("Details", "Sport", new { id = eventType.SportId });
        }
	}
}
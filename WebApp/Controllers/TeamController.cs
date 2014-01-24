using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApp.Models;
using WebApp.Controllers.Base;

namespace WebApp.Controllers
{
    public class TeamController : BaseController
    {

        // GET: /Team/
        public ActionResult Index(int? sportid)
        {
            if (sportid == null)
            {
                var result = this.DataContext.Sports.ToList();
                return View("GeneralIndex", result);
            }
            var sport = DataContext.Sports.Find(sportid);
            if (sport == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(DataContext.Teams.Where(x => x.SportId == sportid).ToList());
        }

        [ChildActionOnly]
        public ActionResult Count(int? sportid)
        {
            if (sportid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var sport = DataContext.Sports.Find(sportid);
            if (sport == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return Content(DataContext.Teams.Count(x => x.SportId == sportid).ToString());
        }

        private Team SelectSingle(int id)
        {
            return DataContext.Teams.Include("Properties").SingleOrDefault(x=>x.Id==id);
        }

        // GET: /Team/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var team = SelectSingle(id.Value);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // GET: /Team/Create
        public ActionResult Create(int? sportid)
        {
            if (sportid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var sport = DataContext.Sports.Find(sportid);
            if (sport == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var team = new Team {SportId = sportid.Value, Properties = new List<TeamProperty>()};
            DataContext.TeamPropertyTypes.Where(x => x.SportId == sportid).ToList().ForEach(x => team.Properties.Add(new TeamProperty { TeamPropertyType = x, TeamPropertyTypeId = x.Id }));
            return View(team);
        }

        // POST: /Team/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Team team)
        {
            if (ModelState.IsValid)
            {
                DataContext.Participants.Add(team);
                DataContext.SaveChanges();
                return RedirectToAction("Index", new { sportid = team.SportId });
            }
            team.Properties.ForEach(x => x.TeamPropertyType = DataContext.TeamPropertyTypes.SingleOrDefault(y => x.TeamPropertyTypeId == y.Id));
            return View(team);
        }

        // GET: /Team/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var team = DataContext.Teams.Include("Properties").Include("Properties.TeamPropertyType").SingleOrDefault(x => x.Id == id);
            //var Team = this.DataContext.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            team.Properties.ForEach(x => x.TeamPropertyType = DataContext.TeamPropertyTypes.SingleOrDefault(y => x.TeamPropertyTypeId == y.Id));
            var typeIds=team.Properties.Select(x=>x.TeamPropertyTypeId).ToList();
            DataContext.TeamPropertyTypes.Where(x => x.SportId == team.SportId && !typeIds.Contains(x.Id)).ToList().ForEach(x => team.Properties.Add(new TeamProperty { TeamPropertyType = x, TeamPropertyTypeId = x.Id }));
            return View(team);
        }

        // POST: /Team/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Team team)
        {
            if (ModelState.IsValid)
            {
                if (team.Properties != null)
                {
                    team.Properties.ForEach(x => x.TeamId = team.Id);
                }
                //Здесь нет проверок на то, что сущность не отслеживается обжектстэйт менеджером и поэтому в этом случае всё может упасть :) 
                // Я не до конца уверен в чем дело. Потом стоит обвешать проверками.
                // TODO: Add checks if entity is no attached and watched by object state manager.
                var ent = SelectSingle(team.Id);
                DataContext.Entry(ent).CurrentValues.SetValues(team);
                if (team.Properties != null)
                {
                    foreach (var item in team.Properties)
                    {
                        var temp = DataContext.TeamProperties.SingleOrDefault(x => x.Id == item.Id);
                        if (temp != null)
                        {
                            DataContext.Entry(temp).CurrentValues.SetValues(item);
                        }
                        else
                        {
                            DataContext.TeamProperties.Add(item);
                        }
                    }
                }

                DataContext.SaveChanges();
                return RedirectToAction("Index", new { sportid = team.SportId });
            }
            team.Properties.ForEach(x => x.TeamPropertyType = DataContext.TeamPropertyTypes.SingleOrDefault(y => x.TeamPropertyTypeId == y.Id));
            return View(team);
        }


        // POST: /Team/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var team = DataContext.Teams.Find(id);
            var properties = new List<TeamProperty>(team.Properties);
            properties.ForEach(x => DataContext.TeamProperties.Remove(x));
            team.Players.Clear();
            DataContext.Teams.Remove(team);
            DataContext.SaveChanges();
            return RedirectToAction("Index", new { sportid = team.SportId });
        }
    }
}

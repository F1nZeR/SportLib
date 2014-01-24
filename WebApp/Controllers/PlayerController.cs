using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;
using WebApp.Data;
using WebApp.Controllers.Base;

namespace WebApp.Controllers
{
    public class PlayerController : BaseController
    {

        // GET: /Player/
        public ActionResult Index(int? sportid)
        {
            if (sportid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sport sport = DataContext.Sports.Find(sportid);
            if (sport == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(DataContext.Players.Where(x => x.SportId == sportid).ToList());
        }

        [ChildActionOnly]
        public ActionResult ShowFromTeam(int teamId)
        {
            var team = DataContext.Teams.Find(teamId);
            if (team == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.TeamId = teamId;
            return PartialView(DataContext.Players.Where(x => x.Teams.Select(y=>y.Id).Contains(teamId)).ToList());
        }

        [ChildActionOnly]
        public ActionResult Count(int? sportid)
        {
            if (sportid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sport sport = DataContext.Sports.Find(sportid);
            if (sport == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return Content(DataContext.Players.Count(x => x.SportId == sportid).ToString());
        }

        private Player SelectSingle(int id)
        {
            return DataContext.Players.Include("Properties").SingleOrDefault(x=>x.Id==id);
        }

        // GET: /Player/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = SelectSingle(id.Value);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        public ActionResult AddExisting(int teamId)
        {
            var team = DataContext.Teams.Single(x => x.Id == teamId);
            var teamPlayers = team.Players.Select(x => x.Id).ToList();
            var players = DataContext.Players.Where(x => x.SportId == team.SportId && !teamPlayers.Contains(x.Id)).ToList();
            ViewBag.TeamId = teamId;
            return View(players);
        }

        [HttpPost]
        public ActionResult AddExisting(int playerId, int teamId)
        {
            var team = DataContext.Teams.Single(x => x.Id == teamId);
            var player = DataContext.Players.Single(x => x.Id == playerId);
            player.Teams.Add(team);
            DataContext.SaveChanges();

            return RedirectToAction("AddExisting", new{teamId});
        }

        // GET: /Player/Create
        public ActionResult Create(int? sportid, int? teamid)
        {
            if (sportid == null && teamid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player;

            if(teamid!=null)
            {
                var team = DataContext.Teams.Find(teamid);
                if (team == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                player = new Player {SportId = team.SportId, Teams = new List<Team> {team}};
            }
            else
            {
                var sport = DataContext.Sports.Find(sportid);
                if (sport == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                player = new Player { SportId = sportid.Value };
            }
           
            
            player.Properties = new List<PlayerProperty>();
            player.BirthDate = DateTime.Now;
            DataContext.PlayerPropertyTypes.Where(x => x.SportId == player.SportId).ToList().ForEach(x => player.Properties.Add(new PlayerProperty { PlayerPropertyType = x, PlayerPropertyTypeId = x.Id }));
            return View(player);
        }

        // POST: /Player/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Player player,int? teamid)
        {
            if (ModelState.IsValid)
            {
                var team = DataContext.Teams.Find(teamid);
                player.Teams = new List<Team> {team};
                DataContext.Participants.Add(player);
                DataContext.SaveChanges();
                if (teamid!=null)
                {
                    return RedirectToAction("Details", "Team", new { id = teamid });
                }
                else
                {
                    return RedirectToAction("Details", "Player", new { id = player.Id});
                }
            }
            player.Properties.ForEach(x => x.PlayerPropertyType = DataContext.PlayerPropertyTypes.SingleOrDefault(y => x.PlayerPropertyTypeId == y.Id));
            return View(player);
        }

        // GET: /Player/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var player = DataContext.Players.Include("Properties").Include("Properties.PlayerPropertyType").SingleOrDefault(x => x.Id == id);
            //var player = this.DataContext.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            player.Properties.ForEach(x => x.PlayerPropertyType = DataContext.PlayerPropertyTypes.SingleOrDefault(y => x.PlayerPropertyTypeId == y.Id));
            var typeIds=player.Properties.Select(x=>x.PlayerPropertyTypeId).ToList();
            DataContext.PlayerPropertyTypes.Where(x => x.SportId == player.SportId && !typeIds.Contains(x.Id)).ToList().ForEach(x => player.Properties.Add(new PlayerProperty { PlayerPropertyType = x, PlayerPropertyTypeId = x.Id }));
            return View(player);
        }

        // POST: /Player/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Player player)
        {
            if (ModelState.IsValid)
            {
                if (player.Properties != null)
                {
                    player.Properties.ForEach(x => x.PlayerId = player.Id);
                }
                //Здесь нет проверок на то, что сущность не отслеживается обжектстэйт менеджером и поэтому в этом случае всё может упасть :) 
                // Я не до конца уверен в чем дело. Потом стоит обвешать проверками.
                // TODO: Add checks if entity is no attached and watched by object state manager.
                var ent = SelectSingle(player.Id);
                DataContext.Entry(ent).CurrentValues.SetValues(player);
                if (player.Properties != null)
                {
                    foreach (var item in player.Properties)
                    {
                        var temp = DataContext.PlayerProperties.SingleOrDefault(x => x.Id == item.Id);
                        if (temp != null)
                        {
                            DataContext.Entry(temp).CurrentValues.SetValues(item);
                        }
                        else
                        {
                            DataContext.PlayerProperties.Add(item);
                        }
                    }
                }
                
                DataContext.SaveChanges();
                return RedirectToAction("Index", new { sportid = player.SportId });
            }
            player.Properties.ForEach(x => x.PlayerPropertyType = DataContext.PlayerPropertyTypes.SingleOrDefault(y => x.PlayerPropertyTypeId == y.Id));
            return View(player);
        }


        // POST: /Player/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var player = DataContext.Players.Find(id);
            var properties = new List<PlayerProperty>(player.Properties);
            properties.ForEach(x => DataContext.PlayerProperties.Remove(x));
            var teams = player.Teams;
            teams.ForEach(x => x.Players.Remove(player));
            var gamesParts = DataContext.GameParticipantPlayers.Where(x => x.PlayerId == player.Id);
            DataContext.GameParticipantPlayers.RemoveRange(gamesParts);
            DataContext.Players.Remove(player);

            DataContext.SaveChanges();
            return RedirectToAction("Index", new { sportid = player.SportId });
        }

        [HttpPost]
        public ActionResult DeleteFromTeam(int playerId, int teamId)
        {
            var player = DataContext.Players.Single(x => x.Id == playerId);
            var team = DataContext.Teams.Single(x => x.Id == teamId);
            player.Teams.Remove(team);
            DataContext.SaveChanges();
            return RedirectToAction("Details", "Team", new {id = teamId});
        }
    }
}

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
    public class TournamentController : BaseController
    {
        public ActionResult Index(int? sportId)
        {
            if(sportId == null)
            {
                var result = this.DataContext.Sports.ToList();
                return View("GeneralIndex", result);
            }
            var tournaments =
                DataContext.Tournaments.Where(x => x.SportId == sportId.Value).OrderBy(x => x.IsFriendlyTournament).ToList();
            
            ViewBag.Sport = DataContext.Sports.Single(x => x.Id == sportId);
            return View(tournaments);
        }

        public ActionResult Details(int id)
        {
            var tournament = DataContext.Tournaments.SingleOrDefault(x => x.Id == id);
            if (tournament == null) return HttpNotFound();
            return View(tournament);
        }

        [ChildActionOnly]
        public ActionResult GetSports()
        {
            var sports = DataContext.Sports.ToList();
            return PartialView("_GetSports", sports);
        }

        [Authorize(Roles = "Admin, Editor")]
        public ActionResult Create()
        {
            ViewBag.Sports = DataContext.Sports.ToList();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Editor")]
        public ActionResult Create([Bind(Exclude = "Id")] Tournament model)
        {
            if (ModelState.IsValid)
            {
                model.IsFriendlyTournament = false;
                DataContext.Tournaments.Add(model);
                DataContext.SaveChanges();
                return RedirectToAction("Index", new {sportId = model.SportId});
            }

            ViewBag.Sports = DataContext.Sports.ToList();
            return View(model);
        }

        [Authorize(Roles = "Admin, Editor")]
        public ActionResult Edit(int id)
        {
            var tournament = DataContext.Tournaments.SingleOrDefault(x => x.Id == id);
            if (tournament == null) return View("Error");

            ViewBag.Sports = DataContext.Sports.ToList();
            return View(tournament);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Editor")]
        public ActionResult Edit(Tournament model)
        {
            if (ModelState.IsValid)
            {
                DataContext.Entry(model).State = EntityState.Modified;
                DataContext.SaveChanges();
                return RedirectToAction("Index", new { sportId = model.SportId });
            }
            ViewBag.Sports = DataContext.Sports.ToList();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Editor")]
        public ActionResult Delete(int id)
        {
            var tourn = DataContext.Tournaments.SingleOrDefault(x => x.Id == id);
            if (tourn == null) return View("Error");

            tourn.TournamentParticipants.Clear();
            tourn.Games.Clear();
            DataContext.Tournaments.Remove(tourn);
            DataContext.SaveChanges();
            return RedirectToAction("Index", new { sportId = tourn.SportId });
        }

        [Authorize(Roles = "Admin, Editor")]
        public ActionResult EditParticipants(int id)
        {
            var sport = DataContext.Tournaments.Single(x => x.Id == id).Sport;

            ViewBag.TournamentId = id;
            var currentPartic = DataContext.TournamentParticipants.Where(x => x.TournamentId == id).Select(x => x.ParticipantId);
            if (sport.IsTeamSport)
            {
                var teams =
                    DataContext.Teams.Where(
                        x => x.SportId == sport.Id && !currentPartic.Contains(x.Id)).Select(x => (Participant)x).ToList();
                return View(teams);
            }
            else
            {
                var players =
                    DataContext.Players.Where(
                        x => x.SportId == sport.Id && !currentPartic.Contains(x.Id)).Select(x => (Participant)x).ToList();
                return View(players);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Editor")]
        public ActionResult AddParticipantToTournament(int tournId, int particId)
        {
            var existing =
                DataContext.TournamentParticipants.Any(x => x.TournamentId == tournId && x.ParticipantId == particId);
            if (existing) return Content("error");

            DataContext.TournamentParticipants.Add(new TournamentParticipant
            {
                ParticipantId = particId,
                TournamentId = tournId
            });
            DataContext.SaveChanges();
            return TournamentParticipants(tournId);
        }

        [HttpPost]
        public ActionResult RemoveParticipantFromTournament(int tournId, int particId)
        {
            var partic = DataContext.TournamentParticipants.SingleOrDefault(
                    x => x.TournamentId == tournId && x.ParticipantId == particId);
            if (partic == null) return Content("error");

            DataContext.TournamentParticipants.Remove(partic);
            DataContext.SaveChanges();
            return TournamentParticipants(tournId);
        }

        [ChildActionOnly]
        public ActionResult TournamentParticipants(int id, bool isEditable = true)
        {
            ViewBag.IsEditable = isEditable;
            var tournament = DataContext.Tournaments.Single(x => x.Id == id);
            var participIds =
                DataContext.TournamentParticipants.Where(x => x.TournamentId == id).Select(x => x.ParticipantId);

            ViewBag.TournamentId = id;
            if (tournament.Sport.IsTeamSport)
            {
                var particips = DataContext.Teams.Where(x => participIds.Contains(x.Id)).ToList();
                return PartialView("_TournamentParticipantsTeams", particips);
            }
            else
            {
                var particips = DataContext.Players.Where(x => participIds.Contains(x.Id)).ToList();
                return PartialView("_TournamentParticipantsPlayers", particips);
            }
        }

        [ChildActionOnly]
        public ActionResult GameList(int id)
        {
            var result = DataContext.Games.Where(x => x.TournamentId == id).ToList();
            return PartialView(result);
        }
	}
}
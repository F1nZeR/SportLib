using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebApp.Controllers.Base;
using WebApp.Models;
using WebApp.Models.ViewModels;

namespace WebApp.Controllers
{
    public class GameController : BaseController
    {
        public ActionResult Index(int sportId, int tournId)
        {
            var games = DataContext.Games.Where(x => x.SportId == sportId && x.TournamentId == tournId);

            ViewBag.TournamentId = tournId;
            ViewBag.SportId = sportId;
            var resGames = games.OrderByDescending(x => x.Date).ToList();
            var result = resGames.Select(x => new GamePreviewViewModel(x, DataContext)).ToList();
            return View(result);
        }

        public ActionResult Details(int id)
        {
            var game = DataContext.Games.SingleOrDefault(x => x.Id == id);
            if (game == null) return HttpNotFound();

            return View(new GamePreviewViewModel(game, DataContext));
        }

        [Authorize(Roles = "Admin, Editor")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var game = DataContext.Games.SingleOrDefault(x => x.Id == id);
            if (game == null) return HttpNotFound();
            var sportId = game.SportId;
            var tournId = game.TournamentId;
            
            DataContext.Events.RemoveRange(game.Events);
            foreach (var gPartic in game.GameParticipants)
            {
                foreach (var gameParticipantPlayer in gPartic.GameParticipantPlayers)
                {
                    DataContext.GameParticipantPlayerProps.RemoveRange(gameParticipantPlayer.GameParticipantPlayerProps);
                }
                DataContext.GameParticipantPlayers.RemoveRange(gPartic.GameParticipantPlayers);
            }
            DataContext.GameParticipants.RemoveRange(game.GameParticipants);
            DataContext.Games.Remove(game);

            DataContext.SaveChanges();
            return RedirectToAction("Index", new {sportId, tournId});
        }

        [Authorize(Roles = "Admin, Editor")]
        public ActionResult Create(int sportId, int tournId)
        {
            var tournaments = DataContext.Tournaments.Where(x => x.SportId == sportId).OrderBy(x => x.IsFriendlyTournament).ToList();
            ViewBag.Tournaments = tournaments;
            ViewBag.SportId = sportId;
            ViewBag.TournamentId = tournId;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Editor")]
        public ActionResult Create([Bind(Exclude = "Id")] Game model)
        {
            if (ModelState.IsValid)
            {
                DataContext.Games.Add(model);
                DataContext.SaveChanges();
                return RedirectToAction("EditParticipants", new {id = model.Id});
            }

            var tournaments = DataContext.Tournaments.Where(x => x.SportId == model.SportId).OrderBy(x => x.IsFriendlyTournament).ToList();
            ViewBag.Tournaments = tournaments;
            ViewBag.SportId = model.SportId;
            ViewBag.TournamentId = model.TournamentId;
            return View(model);
        }

        [Authorize(Roles = "Admin, Editor")]
        public ActionResult Edit(int id)
        {
            var game = DataContext.Games.SingleOrDefault(x => x.Id == id);
            if (game == null) return View("Error");

            var tournaments = DataContext.Tournaments.Where(x => x.SportId == game.SportId).OrderBy(x => x.IsFriendlyTournament).ToList();
            ViewBag.Tournaments = tournaments;

            ViewBag.Participants = game.GameParticipants.Select(x => x.Participant).ToList();
            return View(game);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Editor")]
        public ActionResult Edit(Game model)
        {
            if (ModelState.IsValid)
            {
                DataContext.Entry(model).State = EntityState.Modified;
                DataContext.SaveChanges();
                return RedirectToAction("Details", new { id = model.Id });
            }

            var tournaments = DataContext.Tournaments.Where(x => x.SportId == model.SportId).OrderBy(x => x.IsFriendlyTournament).ToList();
            ViewBag.Tournaments = tournaments;

            var game = DataContext.Games.SingleOrDefault(x => x.Id == model.Id);
            if (game == null) return View("Error");
            ViewBag.Participants = game.GameParticipants.Select(x => x.Participant).ToList();
            return View(model);
        }

        private List<int> AvailableParticipiantsForGame(int gameId)
        {
            var game = DataContext.Games.Single(x => x.Id == gameId);
            if (game.Tournament.IsFriendlyTournament)
            {
                return DataContext.Participants.Where(x => x.SportId == game.SportId).Select(x => x.Id).ToList();
            }
            else
            {
                return
                    DataContext.TournamentParticipants.Where(x => x.TournamentId == game.TournamentId)
                        .Select(x => x.ParticipantId).ToList();
            }
        }

        [Authorize(Roles = "Admin, Editor")]
        public ActionResult EditParticipants(int id)
        {
            ViewBag.GameId = id;
            var game = DataContext.Games.Single(x => x.Id == id);
            var gameParticips = game.GameParticipants.Select(x => x.ParticipantId).ToList();
            var sport = game.Sport;
            ViewBag.GameId = id;
            var avlbl = AvailableParticipiantsForGame(id);
            if (sport.IsTeamSport)
            {
                var teams =
                    DataContext.Teams.Where(
                        x => x.SportId == sport.Id && avlbl.Contains(x.Id) && !gameParticips.Contains(x.Id))
                        .Select(x => (Participant) x)
                        .ToList();
                return View(teams);
            }
            else
            {
                var players =
                    DataContext.Players.Where(
                        x => x.SportId == sport.Id && avlbl.Contains(x.Id) && !gameParticips.Contains(x.Id))
                        .Select(x => (Participant) x)
                        .ToList();
                return View(players);
            }
        }

        [ChildActionOnly]
        public ActionResult GetGameParticipants(int id, bool ed = true)
        {
            // ed - Editable
            ViewBag.Editable = ed;
            var game = DataContext.Games.Single(x => x.Id == id);
            var partic = DataContext.GameParticipants.Where(x => x.GameId == id).ToList();
            var particips = partic.Select(x => x.ParticipantId);
            if (game.Sport.IsTeamSport)
            {
                var teams = DataContext.Teams.Where(x => particips.Contains(x.Id)).ToList();
                var res = GameTeamViewModel.GetModels(partic, teams, DataContext);
                return PartialView("_GameTeams", res);
            }
            else
            {
                var players = DataContext.Players.Where(x => particips.Contains(x.Id)).ToList();
                var res = GamePlayerViewModel.GetModels(partic, players);
                ViewBag.IsDeleteEnabled = true; // можно удалять только если не командный вид спорта
                return PartialView("_GamePlayers", res);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Editor")]
        public ActionResult AddParticipantToGame(int particId, int gameId)
        {
            var game = DataContext.Games.Include("Sport").Single(x => x.Id == gameId);
            if (game.GameParticipants.Any(x => x.ParticipantId == particId)) return Content("Уже добавлен");
            if (game.GameParticipants.Count == game.Sport.SidesCountMax) return Content("Превышен лимит команд");

            var participant = new GameParticipant
            {
                GameId = gameId,
                ParticipantId = particId
            };
            DataContext.GameParticipants.Add(participant);

            if (game.Sport.IsTeamSport)
            {
                var team = DataContext.Teams.Single(x => x.Id == particId);
                foreach (var player in team.Players)
                {
                    var particPlayer = new GameParticipantPlayer
                    {
                        Player = player,
                        GameParticipant = participant,
                    };
                    DataContext.GameParticipantPlayers.Add(particPlayer);

                    // свойства
                    var props = player.Properties.Where(x => x.PlayerPropertyType.IsDependsOnGame);
                    foreach (var playerProperty in props)
                    {
                        DataContext.GameParticipantPlayerProps.Add(new GameParticipantPlayerProp
                        {
                            GameParticipantPlayer = particPlayer,
                            PlayerPropertyType = playerProperty.PlayerPropertyType,
                            PropValue = playerProperty.Value
                        });
                    }
                }
            }
            else
            {
                var player = DataContext.Players.Single(x => x.Id == particId);
                var particPlayer = new GameParticipantPlayer
                {
                    Player = player,
                    GameParticipant = participant,
                };
                DataContext.GameParticipantPlayers.Add(particPlayer);
                var props = player.Properties.Where(x => x.PlayerPropertyType.IsDependsOnGame);
                foreach (var playerProperty in props)
                {
                    DataContext.GameParticipantPlayerProps.Add(new GameParticipantPlayerProp
                    {
                        GameParticipantPlayer = particPlayer,
                        PlayerPropertyType = playerProperty.PlayerPropertyType,
                        PropValue = playerProperty.Value
                    });
                }
            }
            DataContext.SaveChanges();

            return GetGameParticipants(gameId);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Editor")]
        public ActionResult DeleteParticipant(int id)
        {
            var participant = DataContext.GameParticipants.Single(x => x.Id == id);
            var game = participant.Game;
            if (game.WinnerId == participant.ParticipantId)
            {
                game.WinnerId = null;
                DataContext.Entry(game).State = EntityState.Modified;
            }

            DataContext.GameParticipantPlayers.RemoveRange(participant.GameParticipantPlayers);
            DataContext.GameParticipants.Remove(participant);
            DataContext.SaveChanges();
            return GetGameParticipants(game.Id);
        }

        [ChildActionOnly]
        public ActionResult ShowForTeam(int teamId)
        {
            var result = DataContext.Games.Include("Tournament").Include("GameParticipants").ToList().Where(x => x.GameParticipants.Find(y => y.ParticipantId == teamId) != null).ToList();
            return PartialView(result);
        }

        [ChildActionOnly]
        public ActionResult ShowForPlayer(int id)
        {
            var result =
                DataContext.Games.Include("Tournament")
                    .Include("GameParticipants")
                    .ToList()
                    .Where(
                        x => x.GameParticipants.Any(y => y.GameParticipantPlayers.Select(z => z.PlayerId).Contains(id)))
                    .ToList();
            return PartialView(result);
        }

        [ChildActionOnly]
        public ActionResult ShowForSport(int id)
        {
            var result =
               DataContext.Games.Include("Tournament")
                   .Include("GameParticipants")
                   .ToList()
                   .Where(
                       x => x.SportId == id)
                   .ToList();
            return PartialView(result);
        }
    }
}

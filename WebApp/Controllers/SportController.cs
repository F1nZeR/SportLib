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
    public class SportController : BaseController
    {
        //
        // GET: /Sport/
        public ActionResult Index()
        {
            var sports = DataContext.Sports.ToList();
            return View(sports);
        }

        public ActionResult Filters()
        {
            return PartialView(DataContext.Sports.ToList());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View(new Sport());
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Exclude = "Id")] Sport model)
        {
            if (ModelState.IsValid)
            {
                if (DataContext.Sports.Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "Спорт с таким названием уже имеется!");
                    return View(model);
                }
                DataContext.Sports.Add(model);
                #region системные события
                if (model.TimePeriodCount > 1)
                {
                    for (int i = 1; i <= model.TimePeriodCount; i++)
                    {
                        DataContext.EventTypes.Add(new EventType
                        {
                            IsSignifForProtocol = true,
                            IsSystemEventType = true,
                            Sport = model,
                            Name = string.Format("Начало: {0} {1}", i, model.TimePeriodName)
                        });

                        DataContext.EventTypes.Add(new EventType
                        {
                            IsSignifForProtocol = true,
                            IsSystemEventType = true,
                            Sport = model,
                            Name = string.Format("Конец: {0} {1}", i, model.TimePeriodName)
                        });
                    }
                }
                else
                {
                    DataContext.EventTypes.Add(new EventType
                    {
                        IsSignifForProtocol = true,
                        IsSystemEventType = true,
                        Sport = model,
                        Name = "Начало игры"
                    });

                    DataContext.EventTypes.Add(new EventType
                    {
                        IsSignifForProtocol = true,
                        IsSystemEventType = true,
                        Sport = model,
                        Name = "Конец игры"
                    });
                }
                DataContext.EventTypes.Add(new EventType
                {
                    IsSignifForProtocol = true,
                    IsSystemEventType = true,
                    Sport = model,
                    Name = "Дополнительное время"
                });
                #endregion

                DataContext.Tournaments.Add(new Tournament
                {
                    IsFriendlyTournament = true,
                    Name = "Товарищеские матчи",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    Description = null,
                    Place = null
                });
                
                DataContext.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Исправьте ошибки");
            }
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var sport = DataContext.Sports.SingleOrDefault(x => x.Id == id);
            if (sport == null) return View("Error");

            var viewModel = new SportTotalEditViewModel
            {
                Sport = sport,
                EventTypes = sport.EventTypes.ToList(),
                PlayerPropertyTypes = sport.PlayerPropertyTypes.ToList(),
                TeamPropertyTypes = sport.TeamPropertyTypes.ToList()
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            var sport = DataContext.Sports.SingleOrDefault(x => x.Id == id);
            if (sport == null) return View("Error");

            return View(sport);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public ActionResult Edit(Sport model)
        {
            if (ModelState.IsValid)
            {
                DataContext.Sports.Attach(model);
                DataContext.Entry(model).State = EntityState.Modified;
                DataContext.SaveChanges();
                return RedirectToAction("Details", new {id = model.Id});
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
            var sport = DataContext.Sports.SingleOrDefault(x => x.Id == id);
            if (sport == null) return HttpNotFound();

            DataContext.Sports.Remove(sport);
            var teams = DataContext.Teams.Where(x => x.SportId == id).ToList();
            teams.ForEach(x => x.Players.Clear());
            DataContext.Teams.RemoveRange(teams);
            DataContext.Participants.RemoveRange(DataContext.Participants.Where(x => x.SportId == id));
            DataContext.Players.RemoveRange(DataContext.Players.Where(x => x.SportId == id));
            DataContext.GameParticipants.RemoveRange(DataContext.GameParticipants.Where(x => x.Game.SportId == id));
            DataContext.GameParticipantPlayers.RemoveRange(DataContext.GameParticipantPlayers.Where(x => x.Player.SportId == id));
            DataContext.Games.RemoveRange(DataContext.Games.Where(x => x.SportId == id));
            DataContext.TournamentParticipants.RemoveRange(DataContext.TournamentParticipants.Where(x => x.Tournament.SportId == id));
            DataContext.Tournaments.RemoveRange(DataContext.Tournaments.Where(x => x.SportId == id));
            DataContext.PlayerProperties.RemoveRange(DataContext.PlayerProperties.Where(x => x.Player.SportId == id));
            DataContext.GameParticipantPlayers.RemoveRange(DataContext.GameParticipantPlayers.Where(x => x.Player.SportId == id));
            DataContext.GameParticipantPlayerProps.RemoveRange(DataContext.GameParticipantPlayerProps.Where(x => x.PlayerPropertyType.SportId == id));
            DataContext.TeamProperties.RemoveRange(DataContext.TeamProperties.Where(x => x.TeamPropertyType.SportId == id));
            DataContext.TeamPropertyTypes.RemoveRange(DataContext.TeamPropertyTypes.Where(x => x.SportId == id));

            DataContext.SaveChanges();
            return RedirectToAction("Index");
        }
	}
}
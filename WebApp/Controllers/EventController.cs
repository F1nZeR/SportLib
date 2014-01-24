using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using WebApp.Controllers.Base;
using WebApp.Helpers;
using WebApp.Hubs;
using WebApp.Models;
using WebApp.Models.ViewModels;

namespace WebApp.Controllers
{
    public class EventController : BaseController
    {
        [HttpPost]
        public ActionResult Create(Event model)
        {
            if (ModelState.IsValid)
            {
                model.Count = model.Count == 0 ? 1 : model.Count;
                DataContext.Events.Add(model);
                DataContext.SaveChanges();

                var curEvent = DataContext.Events.Include("GameParticipantPlayer").Include("EventType").Single(x => x.Id == model.Id);
                var context = GlobalHost.ConnectionManager.GetHubContext<GameLogHub>();
                if (curEvent.EventType.IsSignifForProtocol)
                {
                    var result = RenderHelper.RenderPartialToString("_SingleEvent", new EventViewModel(curEvent), ControllerContext);
                    context.Clients.All.showEvent(result);
                }
                if (!curEvent.EventType.IsSystemEventType)
                {
                    var protocol = new ProtocolViewModel(DataContext, model.GameId);
                    var result = RenderHelper.RenderPartialToString("_GameProtocol", protocol, ControllerContext);
                    context.Clients.All.showProtocol(result);
                }
            }
            return GetEventEditor(model.EventTypeId, model.GameId);
        }

        public ActionResult GetEventEditor(int eventTypeId, int gameId)
        {
            var eventType = DataContext.EventTypes.Single(x => x.Id == eventTypeId);
            var sport = eventType.Sport;
            ViewBag.Sport = sport;

            var game = DataContext.Games.Single(x => x.Id == gameId);
            var players = new List<SelectListItem>();
            game.GameParticipants.ForEach(
                x =>
                    players.AddRange(
                        x.GameParticipantPlayers.Select(
                            z => new SelectListItem {Text = z.Player.Name, Value = z.Id.ToString()})));
            ViewBag.Players = players;


            ViewBag.EventType = eventType;
            var gameEvent = new Event {EventTypeId = eventTypeId, GameId = gameId};
            ModelState.Clear();
            return PartialView("_EventEditor", gameEvent);
        }

        public ActionResult GetGameLog(int gameId)
        {
            var events =
                DataContext.Events.Where(x => x.GameId == gameId && x.EventType.IsSignifForProtocol)
                    .OrderByDescending(x => x.Id)
                    .ToList().Select(x => new EventViewModel(x));
            return PartialView("_GameLog", events);
        }

        public ActionResult GetProtocol(int gameId)
        {
            var protocol = new ProtocolViewModel(DataContext, gameId);
            return PartialView("_GameProtocol", protocol);
        }

        [System.Web.Mvc.Authorize(Roles = "Admin, Editor")]
        public ActionResult Manage(int gameId)
        {
            var events =
                DataContext.Events.Where(x => x.GameId == gameId)
                    .OrderByDescending(x => x.Id)
                    .ToList()
                    .Select(x => new EventViewModel(x));
            ViewBag.GameId = gameId;
            return View(events);
        }

        [System.Web.Mvc.Authorize(Roles = "Admin, Editor")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var ev = DataContext.Events.Include("EventType").Single(x => x.Id == id);
            var gameId = ev.GameId;
            var isSystem = ev.EventType.IsSystemEventType;
            var isSignificant = ev.EventType.IsSignifForProtocol;
            DataContext.Events.Remove(ev);
            DataContext.SaveChanges();

            var context = GlobalHost.ConnectionManager.GetHubContext<GameLogHub>();
            if (!isSystem)
            {
                var protocol = new ProtocolViewModel(DataContext, gameId);
                var result = RenderHelper.RenderPartialToString("_GameProtocol", protocol, ControllerContext);
                context.Clients.All.showProtocol(result);
            }
            if (isSignificant)
            {
                context.Clients.All.updateSingleLog(id, "");
            }
            return RedirectToAction("Manage", new {gameId});
        }

        [System.Web.Mvc.Authorize(Roles = "Admin, Editor")]
        public ActionResult Edit(int id)
        {
            var ev = DataContext.Events.Single(x => x.Id == id);
            var eventType = ev.EventType;
            var sport = eventType.Sport;
            ViewBag.Sport = sport;

            var game = ev.Game;
            var players = new List<SelectListItem>();
            game.GameParticipants.ForEach(
                x =>
                    players.AddRange(
                        x.GameParticipantPlayers.Select(
                            z => new SelectListItem { Text = z.Player.Name, Value = z.Id.ToString() })));
            ViewBag.Players = players;
            return View(ev);
        }

        [System.Web.Mvc.Authorize(Roles = "Admin, Editor")]
        [HttpPost]
        public ActionResult Edit(Event model)
        {
            if (ModelState.IsValid)
            {
                DataContext.Entry(model).State = EntityState.Modified;
                DataContext.SaveChanges();

                var curEvent = DataContext.Events.Include("EventType").Single(x => x.Id == model.Id);
                var context = GlobalHost.ConnectionManager.GetHubContext<GameLogHub>();
                if (!curEvent.EventType.IsSystemEventType)
                {
                    var protocol = new ProtocolViewModel(DataContext, model.GameId);
                    var result = RenderHelper.RenderPartialToString("_GameProtocol", protocol, ControllerContext);
                    context.Clients.All.showProtocol(result);
                }
                if (model.EventType.IsSignifForProtocol)
                {
                    var result = RenderHelper.RenderPartialToString("_SingleEvent", new EventViewModel(curEvent), ControllerContext);
                    context.Clients.All.updateSingleLog(model.Id, result);
                }
                return RedirectToAction("Manage", new {gameId = model.GameId});
            }

            var ev = DataContext.Events.Single(x => x.Id == model.Id);
            var eventType = ev.EventType;
            var sport = eventType.Sport;
            ViewBag.Sport = sport;

            var game = ev.Game;
            var players = new List<SelectListItem>();
            game.GameParticipants.ForEach(
                x =>
                    players.AddRange(
                        x.GameParticipantPlayers.Select(
                            z => new SelectListItem { Text = z.Player.Name, Value = z.Id.ToString() })));
            ViewBag.Players = players;
            return View(model);
        }
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Controllers.Base;
using WebApp.Helpers;
using WebApp.Models.Statistic;
using WebApp.Models.ViewModels;

namespace WebApp.Controllers
{
    public class StatisticController : BaseController
    {
        public ActionResult Index()
        {
            var sports = DataContext.Sports.ToList();
            ViewBag.SportList = sports;
            return View();
        }

        public ActionResult GetStatTarget(int sportId)
        {
            var sport = DataContext.Sports.Single(x => x.Id == sportId);
            var statTargets = StatTarget.GetStatTargets(sport);
            ViewBag.SportId = sport.Id;

            var eventTypes = DataContext.EventTypes.Where(x => x.SportId == sportId && !x.IsSystemEventType).ToList();
            ViewBag.EventTypes = eventTypes;
            return PartialView("_StatTarget", statTargets);
        }

        public ActionResult GetStatTargetEditor(int sportId, StatTargetType type)
        {
            var sport = DataContext.Sports.Single(x => x.Id == sportId);
            SelectList result;
            List<StatTargetType> contextTypes;
            switch (type)
            {
                case StatTargetType.Players:
                    var players = DataContext.Players.Where(x => x.SportId == sportId).ToList();
                    result = new SelectList(players, "Id", "Name");
                    contextTypes = new List<StatTargetType>();
                    if (sport.IsTeamSport)
                    {
                        contextTypes.Add(StatTargetType.Teams);
                    }
                    contextTypes.Add(StatTargetType.Games);
                    contextTypes.Add(StatTargetType.Tournaments);
                    break;
                case StatTargetType.Teams:
                    var teams = DataContext.Teams.Where(x => x.SportId == sportId).ToList();
                    result = new SelectList(teams, "Id", "Name");
                    contextTypes = new List<StatTargetType> {StatTargetType.Players, StatTargetType.Games, StatTargetType.Tournaments};
                    break;
                default:
                    var games =
                        DataContext.Games.Where(x => x.SportId == sportId).ToList()
                            .Select(x => new GamePreviewViewModel(x, DataContext))
                            .ToList();
                    result = new SelectList(games, "Id", "DisplayName");
                    contextTypes = new List<StatTargetType> {StatTargetType.Players};
                    if (sport.IsTeamSport)
                    {
                        contextTypes.Add(StatTargetType.Teams);
                    }
                    break;
            }
            ViewBag.StatType = type;
            ViewBag.ContextTypes = contextTypes;
            return PartialView("_StatTargetEditor", result);
        }

        public ActionResult GetContextEditor(int sportId, StatTargetType targetType, string elements, StatTargetType contextType)
        {
            ViewBag.ContextType = contextType;
            var selectList = StatisticHelper.GetContextEditor(sportId, targetType, elements.Split(',').Select(int.Parse).ToArray(), contextType, DataContext);
            return PartialView("_ContextEditor", selectList);
        }

        [HttpPost]
        public ActionResult GetStatistic(StatisticRequest model)
        {
            var statistic = StatisticHelper.GetStatistic(model, DataContext);

            var eventTypes = model.TargetEvents == null
                ? DataContext.EventTypes.Where(x => x.SportId == model.SportId && !x.IsSystemEventType).ToList()
                : DataContext.EventTypes.Where(x => model.TargetEvents.Contains(x.Id)).ToList();
            ViewBag.RequestModel = model;
            ViewBag.EventTypes = eventTypes;
            return PartialView("_StatisticResult", statistic);
        }
    }
}
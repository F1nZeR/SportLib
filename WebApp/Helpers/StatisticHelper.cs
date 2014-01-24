using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Data;
using WebApp.Models;
using WebApp.Models.Statistic;
using WebApp.Models.ViewModels;

namespace WebApp.Helpers
{
    public class StatisticHelper
    {
        public static SelectList GetContextEditor(int sportId, StatTargetType targetType, int[] elems,
            StatTargetType contextType, DataContext context)
        {
            SelectList result = null;
            var sport = context.Sports.Single(x => x.Id == sportId);
            switch (targetType)
            {
                case StatTargetType.Players:
                    var players = context.Players.Where(x => elems.Contains(x.Id)).Select(x => x.Id).ToList();
                    switch (contextType)
                    {
                        case StatTargetType.Teams:
                            var teamsWithPlayers =
                                context.GameParticipants.Where(
                                    x => x.GameParticipantPlayers.Any(y => players.Contains(y.PlayerId))).ToList().Select(x => (Team)x.Participant);
                            var teamsPlayersResult = new HashSet<Team>(teamsWithPlayers);
                            result = new SelectList(teamsPlayersResult, "Id", "Name");
                            break;
                        case StatTargetType.Games:
                            var gamesWithPlayer = context.Games.Where(x => x.GameParticipants.Any(
                                z => z.GameParticipantPlayers.Any(y => players.Contains(y.PlayerId)))).ToList()
                                .Select(x => new GamePreviewViewModel(x, context));
                            result = new SelectList(gamesWithPlayer, "Id", "DisplayName");
                            break;
                        case StatTargetType.Tournaments:
                            var tournaments = context.Tournaments.Where(
                                x => x.Games.Any(
                                    y => y.GameParticipants.Any(
                                        z => z.GameParticipantPlayers.Any(f => players.Contains(f.PlayerId)))))
                                .ToList();
                            result = new SelectList(tournaments, "Id", "Name");
                            break;
                    }
                    break;
                case StatTargetType.Teams:
                    var teams = context.Teams.Where(x => elems.Contains(x.Id)).Select(x => x.Id).ToList();
                    switch (contextType)
                    {
                        case StatTargetType.Players:
                            var playersInTeam = context.Players.Where(x => x.Teams.Any(y => teams.Contains(y.Id)));
                            result = new SelectList(playersInTeam, "Id", "Name");
                            break;
                        case StatTargetType.Games:
                            var gamesOfTeam = context.Games.Where(x => x.GameParticipants.Any(y => teams.Contains(y.ParticipantId)))
                                .ToList().Select(x => new GamePreviewViewModel(x, context));
                            result = new SelectList(gamesOfTeam, "Id", "DisplayName");
                            break;
                        case StatTargetType.Tournaments:
                            var tournsWithTeam = context.Tournaments.Where(x => x.Games.Any(
                                y => y.GameParticipants.Any(z => teams.Contains(z.ParticipantId))));
                            result = new SelectList(tournsWithTeam, "Id", "Name");
                            break;
                    }
                    break;
                case StatTargetType.Games:
                    var games = context.Games.Where(x => elems.Contains(x.Id)).Select(x => x.Id).ToList();
                    switch (contextType)
                    {
                        case StatTargetType.Players:
                            var playersInGames = new HashSet<Player>(context.GameParticipantPlayers.Where(
                                x => games.Contains(x.GameParticipant.GameId)).Select(x => x.Player));
                            result = new SelectList(playersInGames, "Id", "Name");
                            break;
                        case StatTargetType.Teams:
                            var teamsInGames = new HashSet<Team>(context.GameParticipants.Where(
                                x => games.Contains(x.GameId)).ToList().Select(x => (Team)x.Participant));
                            result = new SelectList(teamsInGames, "Id", "Name");
                            break;
                    }
                    break;
            }
            return result;
        }

        public static List<Tuple<object, List<float>>> GetStatistic(StatisticRequest request, DataContext context)
        {
            var result = new List<Tuple<object, List<float>>>();
            var sport = context.Sports.Single(x => x.Id == request.SportId);
            var eventTypes = request.TargetEvents == null
                ? context.EventTypes.Where(x => x.SportId == request.SportId && !x.IsSystemEventType).ToList()
                : context.EventTypes.Where(x => request.TargetEvents.Contains(x.Id)).ToList();
            var sortEventPos = eventTypes.FindIndex(x => x.Id == request.SortEvent);

            switch (request.StatType)
            {
                case StatTargetType.Players:
                    result = QueryPlayers(request, context, eventTypes, sortEventPos);
                    break;
                case StatTargetType.Teams:
                    result = QueryTeams(request, context, eventTypes, sortEventPos);
                    break;
                case StatTargetType.Games:
                    result = QueryGames(request, context, eventTypes, sortEventPos);
                    break;
            }
            return result.Where(x => x != null).ToList();
        }

        private static List<Tuple<object, List<float>>> QueryGames(StatisticRequest request, DataContext context, List<EventType> eventTypes, int sortEventPos)
        {
            var result = new List<Tuple<object, List<float>>>();
            var games = context.Games.Include(x => x.Events).Include(x => x.GameParticipants)
                .Where(x => request.StatElements.Contains(x.Id)).ToList();
            var gamesIds = games.Select(x => x.Id).ToList();
            switch (request.ContextType)
            {
                case StatTargetType.Players:
                    var requestContextPlayers = request.ContextElems ?? new HashSet<int>(context.GameParticipantPlayers.Where(
                                x => gamesIds.Contains(x.GameParticipant.GameId)).Select(x => x.PlayerId)).ToArray();
                    var contextPlayers = context.Players.Where(x => requestContextPlayers.Contains(x.Id)).ToList();
                    foreach (var game in games)
                    {
                        result.Add(new Tuple<object, List<float>>(new GamePreviewViewModel(game, context), null));
                        var gameTotal = new List<float[]>();
                        var playersResult = new List<Tuple<object, List<float>>>();
                        var playersInGame = contextPlayers.Where(x => game.GameParticipants.Any(
                            y => y.GameParticipantPlayers.Any(z => z.PlayerId == x.Id))).ToList();
                        foreach (var player in playersInGame)
                        {
                            var countRes = new List<float>();
                            countRes.AddRange(eventTypes.Select(eventType => game.Events.Where(x => x.EventTypeId == eventType.Id &&
                                x.GameParticipantPlayer.PlayerId == player.Id).Sum(x => (float)x.Count)));
                            playersResult.Add(Tuple.Create((object)player, countRes));
                            gameTotal.Add(countRes.ToArray());
                        }

                        result.AddRange(OrderByEventType(playersResult, sortEventPos, request.SortOrder));
                        result.Add(GetAgregateResult(gameTotal, request.AgregateType));
                    }
                    break;
                case StatTargetType.Teams:
                    var requestContextTeams = request.ContextElems ?? new HashSet<int>(context.GameParticipants.Where(
                                x => gamesIds.Contains(x.GameId)).Select(x => x.ParticipantId)).ToArray();
                    var contextTeams = context.Teams.Where(x => requestContextTeams.Contains(x.Id)).ToList();
                    foreach (var game in games)
                    {
                        result.Add(new Tuple<object, List<float>>(new GamePreviewViewModel(game, context), null));
                        var gameTotal = new List<float[]>();
                        var teamsResult = new List<Tuple<object, List<float>>>();
                        var teamsInGame = contextTeams.Where(x => game.GameParticipants.Any(y => y.ParticipantId == x.Id)).ToList();
                        foreach (var team in teamsInGame)
                        {
                            var countRes = new List<float>();
                            var particsOfTeamInGame = game.GameParticipants.Single(x => x.ParticipantId == team.Id)
                                .GameParticipantPlayers.Select(x => x.PlayerId).ToList();
                            countRes.AddRange(eventTypes.Select(eventType => game.Events.Where(x => x.EventTypeId == eventType.Id &&
                                particsOfTeamInGame.Contains(x.GameParticipantPlayer.PlayerId)).Sum(x => (float)x.Count)));
                            teamsResult.Add(Tuple.Create((object)team, countRes));
                            gameTotal.Add(countRes.ToArray());
                        }

                        result.AddRange(OrderByEventType(teamsResult, sortEventPos, request.SortOrder));
                        result.Add(GetAgregateResult(gameTotal, request.AgregateType));
                    }
                    break;
            }
            return result;
        }

        private static List<Tuple<object, List<float>>> QueryTeams(StatisticRequest request, DataContext context, List<EventType> eventTypes, int sortEventPos)
        {
            var result = new List<Tuple<object, List<float>>>();
            var teams = context.Teams.Where(x => request.StatElements.Contains(x.Id)).ToList();
            var teamsIds = teams.Select(x => x.Id).ToList();
            switch (request.ContextType)
            {
                case StatTargetType.Players:
                    var requestContextPlayers = request.ContextElems ?? context.Players.Where(
                        x => x.Teams.Any(y => teamsIds.Contains(y.Id))).Select(x => x.Id).ToArray();
                    var contextPlayers = context.Players.Where(x => requestContextPlayers.Contains(x.Id)).ToList();
                    foreach (var team in teams)
                    {
                        result.Add(new Tuple<object, List<float>>(team, null));
                        var teamTotal = new List<float[]>();
                        var playersResult = new List<Tuple<object, List<float>>>();
                        var playersInTeam = contextPlayers.Where(x => x.Teams.Any(y => y.Id == team.Id)).ToList();
                        foreach (var player in playersInTeam)
                        {
                            var countRes = new List<float>();
                            // игры, где игрок играл в составе этой команды
                            var gamesWithThisPlayer = context.Games.Where(x => x.GameParticipants.Any(
                                y => y.ParticipantId == team.Id && y.GameParticipantPlayers.Any(z => z.PlayerId == player.Id)))
                                .Select(x => x.Id).ToList();
                            var events = context.Events.Where(x => gamesWithThisPlayer.Contains(x.GameId)).ToList();
                            countRes.AddRange(eventTypes.Select(eventType => events.Where(x => x.EventTypeId == eventType.Id &&
                                x.GameParticipantPlayer.PlayerId == player.Id).Sum(x => (float)x.Count)));
                            playersResult.Add(Tuple.Create((object)player, countRes));
                            teamTotal.Add(countRes.ToArray());
                        }

                        result.AddRange(OrderByEventType(playersResult, sortEventPos, request.SortOrder));
                        result.Add(GetAgregateResult(teamTotal, request.AgregateType));
                    }
                    break;
                case StatTargetType.Games:
                    var requestGames = request.ContextElems ?? context.Games.Where(x => x.GameParticipants.Any(
                            y => teamsIds.Contains(y.ParticipantId))).Select(x => x.Id).ToArray();
                    foreach (var team in teams)
                    {
                        result.Add(new Tuple<object, List<float>>(team, null));
                        var teamTotal = new List<float[]>();
                        var gamesResult = new List<Tuple<object, List<float>>>();
                        var gamesWithTeam = context.Games.Include("Events").Include("GameParticipants").Where(x => 
                            requestGames.Contains(x.Id) && x.GameParticipants.Any(y => y.ParticipantId == team.Id)).ToList();
                        foreach (var game in gamesWithTeam)
                        {
                            var countRes = new List<float>();
                            var playersOfTeamForGame = game.GameParticipants.Single(x => x.ParticipantId == team.Id)
                                    .GameParticipantPlayers.Select(x => x.PlayerId).ToList();
                            var eventsForTeam = game.Events.Where(x => x.GameParticipantPlayerId != null && 
                                playersOfTeamForGame.Contains(x.GameParticipantPlayer.PlayerId));
                            countRes.AddRange(eventTypes.Select(eventType => eventsForTeam.Where(x => x.EventTypeId == eventType.Id)
                                .Sum(x => (float)x.Count)));
                            gamesResult.Add(Tuple.Create((object)new GamePreviewViewModel(game, context), countRes));
                            teamTotal.Add(countRes.ToArray());
                        }

                        result.AddRange(OrderByEventType(gamesResult, sortEventPos, request.SortOrder));
                        result.Add(GetAgregateResult(teamTotal, request.AgregateType));
                    }
                    break;
                case StatTargetType.Tournaments:
                    var tournaments = request.ContextElems ?? context.Tournaments.Where(x => x.Games.Any(
                        y => y.GameParticipants.Any(z => teamsIds.Contains(z.ParticipantId))))
                        .Select(x => x.Id).ToArray();
                    foreach (var team in teams)
                    {
                        result.Add(new Tuple<object, List<float>>(team, null));
                        var teamTotal = new List<float[]>();
                        var tournsResult = new List<Tuple<object, List<float>>>();
                        var tournsWithTeam = context.Tournaments.Where(x => tournaments.Contains(x.Id) && 
                            x.Games.Any(y => y.GameParticipants.Any(z => z.ParticipantId == team.Id))).ToList();
                        foreach (var tournament in tournsWithTeam)
                        {
                            var countRes = new List<float>();
                            var gameParticipants = context.GameParticipants.Include("GameParticipantPlayers")
                                .Where(x => x.ParticipantId == team.Id).Select(x => x.Id).ToList();
                            var eventsForTeam = context.Events.Where(x => gameParticipants.Contains(
                                x.GameParticipantPlayer.GameParticipantId)).ToList();
                            countRes.AddRange(eventTypes.Select(eventType => eventsForTeam.Where(x => x.EventTypeId == eventType.Id)
                                .Sum(x => (float)x.Count)));
                            tournsResult.Add(new Tuple<object, List<float>>(tournament, countRes));
                            teamTotal.Add(countRes.ToArray());
                        }
                        result.AddRange(OrderByEventType(tournsResult, sortEventPos, request.SortOrder));
                        result.Add(GetAgregateResult(teamTotal, request.AgregateType));
                    }
                    break;
            }
            return result;
        }

        private static List<Tuple<object, List<float>>> QueryPlayers(StatisticRequest request, DataContext context, List<EventType> eventTypes, int sortEventPos)
        {
            var result = new List<Tuple<object, List<float>>>();
            var players = context.Players.Where(x => request.StatElements.Contains(x.Id)).ToList();
            var playersIds = players.Select(z => z.Id).ToList();
            switch (request.ContextType)
            {
                case StatTargetType.Teams:
                    var requestContextTeams = request.ContextElems ?? context.GameParticipants.Include("GameParticipantPlayers").Where(
                            x => x.GameParticipantPlayers.Any(y => playersIds.Contains(y.PlayerId))).Select(x => x.ParticipantId).ToArray();
                    var teamsWithPlayers =
                        context.GameParticipants.Include("Game").Include("GameParticipantPlayers").Where(
                            x => requestContextTeams.Contains(x.ParticipantId)).ToList();
                    foreach (var player in players)
                    {
                        result.Add(new Tuple<object, List<float>>(player, null));
                        var distinctTeams = new HashSet<Team>(teamsWithPlayers.Where(
                            x => x.GameParticipantPlayers.Any(y => y.PlayerId == player.Id)).Select(x => (Team)x.Participant));
                        var playerTotal = new List<float[]>();
                        var teamsResult = new List<Tuple<object, List<float>>>();
                        foreach (var team in distinctTeams)
                        {
                            var countRes = new List<float>();
                            var gamePartics = teamsWithPlayers.Where(x => x.ParticipantId == team.Id).Select(x => x.Id).ToList();
                            var events = context.Events.Where(x => x.Game.GameParticipants.Any(y => gamePartics.Contains(y.Id))).ToList();
                            countRes.AddRange(eventTypes.Select(eventType => events.Where(x => x.EventTypeId == eventType.Id &&
                                x.GameParticipantPlayer.PlayerId == player.Id).Sum(x => (float)x.Count)));
                            teamsResult.Add(Tuple.Create((object)team, countRes));
                            playerTotal.Add(countRes.ToArray());
                        }
                        result.AddRange(OrderByEventType(teamsResult, sortEventPos, request.SortOrder));
                        result.Add(GetAgregateResult(playerTotal, request.AgregateType));
                    }
                    break;
                case StatTargetType.Games:
                    var requestGames = request.ContextElems ?? context.Games.Where(x => x.GameParticipants.Any(
                            z => z.GameParticipantPlayers.Any(y => playersIds.Contains(y.PlayerId)))).Select(x => x.Id).ToArray();
                    foreach (var player in players)
                    {
                        result.Add(new Tuple<object, List<float>>(player, null));
                        var gamesWithPlayer = context.Games.Include("GameParticipants").Where(x => requestGames.Contains(x.Id)).ToList();
                        var playerTotal = new List<float[]>();
                        var teamsResult = new List<Tuple<object, List<float>>>();
                        foreach (var game in gamesWithPlayer.Where(x => x.GameParticipants.Any(z => z.GameParticipantPlayers.Any(y => playersIds.Contains(y.PlayerId)))))
                        {
                            var countRes = new List<float>();
                            var events = context.Events.Where(x => x.GameId == game.Id).ToList();
                            countRes.AddRange(eventTypes.Select(eventType => events.Where(x => x.EventTypeId == eventType.Id &&
                                x.GameParticipantPlayer.PlayerId == player.Id).Sum(x => (float)x.Count)));
                            teamsResult.Add(Tuple.Create((object)new GamePreviewViewModel(game, context), countRes));
                            playerTotal.Add(countRes.ToArray());
                        }
                        result.AddRange(OrderByEventType(teamsResult, sortEventPos, request.SortOrder));
                        result.Add(GetAgregateResult(playerTotal, request.AgregateType));
                    }
                    break;
                case StatTargetType.Tournaments:
                    var tournaments = request.ContextElems ?? context.Tournaments.Where(x => x.Games.Any(
                        y => y.GameParticipants.Any(z => z.GameParticipantPlayers.Any(f => playersIds.Contains(f.PlayerId)))))
                        .Select(x => x.Id).ToArray();
                    foreach (var player in players)
                    {
                        result.Add(new Tuple<object, List<float>>(player, null));
                        var tourns = context.Tournaments.Include("Games").Where(x => tournaments.Contains(x.Id)).ToList();
                        var playerTotal = new List<float[]>();
                        var tournsResult = new List<Tuple<object, List<float>>>();
                        var tournWithPlayer = tourns.Where(x => x.Games.Any(y => y.GameParticipants.Any(
                            z => z.GameParticipantPlayers.Any(f => f.PlayerId == player.Id))));
                        foreach (var tourn in tournWithPlayer)
                        {
                            var countRes = new List<float>();
                            var gamesWithPlayer = tourn.Games.Select(x => x.Id);
                            var events = context.Events.Where(x => gamesWithPlayer.Contains(x.GameId)).ToList();
                            countRes.AddRange(eventTypes.Select(eventType => events.Where(x => x.EventTypeId == eventType.Id &&
                                x.GameParticipantPlayer.PlayerId == player.Id).Sum(x => (float)x.Count)));
                            tournsResult.Add(new Tuple<object, List<float>>(tourn, countRes));
                            playerTotal.Add(countRes.ToArray());
                        }
                        result.AddRange(OrderByEventType(tournsResult, sortEventPos, request.SortOrder));
                        result.Add(GetAgregateResult(playerTotal, request.AgregateType));
                    }
                    break;
            }
            return result;
        }

        private static Tuple<object, List<float>> GetAgregateResult(List<float[]> values, StatAgregateType type)
        {
            var resultVals = new List<float>();
            var array = CreateRectangularArray(values);
            if (array == null) return null;
            for (int j = 0; j < array.GetLength(1); j++)
            {
                var cur = new List<float>();
                for (int i = 0; i < array.GetLength(0); i++)
                {
                    cur.Add(array[i, j]);
                }
                switch (type)
                {
                    case StatAgregateType.Max:
                        resultVals.Add(cur.Max());
                        break;
                    case StatAgregateType.Min:
                        resultVals.Add(cur.Min());
                        break;
                    case StatAgregateType.Mean:
                        resultVals.Add(cur.Sum()/cur.Count);
                        break;
                    case StatAgregateType.Total:
                        resultVals.Add(cur.Sum());
                        break;
                }
            }
            return new Tuple<object, List<float>>(type.Description(), resultVals);
        }

        private static T[,] CreateRectangularArray<T>(IList<T[]> arrays)
        {
            if (arrays.Count == 0) return null;
            int minorLength = arrays[0].Length;
            var ret = new T[arrays.Count, minorLength];
            for (int i = 0; i < arrays.Count; i++)
            {
                var array = arrays[i];
                for (int j = 0; j < minorLength; j++)
                {
                    ret[i, j] = array[j];
                }
            }
            return ret;
        }

        private static IEnumerable<Tuple<object, List<float>>> OrderByEventType(List<Tuple<object, List<float>>> input,
            int sortEventPos, int sortOrder)
        {
            // sortOrder: 0 desc, 1 asc
            if (input.Count == 0 || sortEventPos == -1) return input;
            if (sortOrder == 0)
            {
                return input.OrderByDescending(x => x.Item2[sortEventPos]).ToList();
            }
            else
            {
                return input.OrderBy(x => x.Item2[sortEventPos]).ToList();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Helpers
{
    public class XmlCreator
    {
        private readonly DataContext _context;

        public XmlCreator(DataContext context)
        {
            _context = context;
        }

        public byte[] GetTournamentXml(int id)
        {
            _context.Configuration.ProxyCreationEnabled = false;
            var tourn = _context.Tournaments.
                Include(x => x.Sport).Include(x => x.Games).Include(x => x.TournamentParticipants).
                Single(x => x.Id == id);

            tourn.Sport.Participants = _context.Participants.Where(x => x.SportId == tourn.SportId).ToList();
            if (tourn.IsFriendlyTournament)
            {
                if (tourn.Sport.IsTeamSport)
                {
                    var allTeams = tourn.Sport.Participants.OfType<Team>().ToList();
                    tourn.TournamentParticipants =
                        allTeams.Select(x => new TournamentParticipant { Id = 0, Participant = x, ParticipantId = x.Id })
                            .ToList();
                }
                else
                {
                    var allPlayers = tourn.Sport.Participants.OfType<Player>().ToList();
                    tourn.TournamentParticipants =
                        allPlayers.Select(x => new TournamentParticipant { Id = 0, Participant = x, ParticipantId = x.Id })
                            .ToList();
                }
            }

            var participants = tourn.TournamentParticipants.Select(x => x.ParticipantId);
            var teams = _context.Teams.Include(x => x.Players).Where(x => participants.Contains(x.Id)).ToList();
            teams.Clear();
            var players = _context.Players.Where(x => participants.Contains(x.Id)).ToList();
            players.Clear();
            var gamePartics = _context.GameParticipants.Include(x => x.GameParticipantPlayers).Where(x => x.Game.TournamentId == id).ToList();
            gamePartics.Clear();
            var games = _context.Games.Where(x => x.TournamentId == tourn.Id && !x.Events.Any()).ToList();
            tourn.Games = games;
            var eventTypes = _context.EventTypes.Where(x => x.SportId == tourn.SportId).ToList();
            eventTypes.Clear();
            _context.Configuration.ProxyCreationEnabled = true;

            if (tourn.TournamentParticipants != null)
            {
                foreach (var tournPartic in tourn.TournamentParticipants)
                {
                    tournPartic.Participant.Sport = null;
                    tournPartic.Tournament = null;
                    var team = tournPartic.Participant as Team;
                    if (team != null)
                    {
                        foreach (var player in team.Players)
                        {
                            player.Teams = null;
                            player.Sport = null;
                            player.Properties = null;
                        }
                    }
                    else
                    {
                        var player = (Player)tournPartic.Participant;
                        player.Properties = null;
                        player.Teams = null;
                        player.Sport = null;
                    }
                }
            }

            // спорт
            tourn.Sport.Participants = null;
            tourn.Sport.Games = null;
            tourn.Sport.PlayerPropertyTypes = null;
            tourn.Sport.Tournaments = null;

            // события
            foreach (var eventType in tourn.Sport.EventTypes)
            {
                eventType.Sport = null;
            }

            // игры
            foreach (var game in tourn.Games)
            {
                game.Events = null;
                game.Tournament = null;
                game.Winner = null;
                game.Sport = null;
                if (game.GameParticipants != null)
                {
                    foreach (var gameParticipant in game.GameParticipants)
                    {
                        gameParticipant.Game = null;
                        gameParticipant.Participant = null;
                        foreach (var gameParticipantPlayer in gameParticipant.GameParticipantPlayers)
                        {
                            gameParticipantPlayer.GameParticipant = null;
                            gameParticipantPlayer.GameParticipantPlayerProps = null;
                            gameParticipantPlayer.Player = null;
                        }
                    }
                }
            }

            return SerializeAnObject(tourn);
        }

        public void ImportTournaments(HttpPostedFileBase file)
        {
            var str = new StreamReader(file.InputStream, Encoding.UTF8).ReadToEnd();
            var xmlSerializer = new XmlSerializer(typeof (Tournament));
            Tournament xmlTourn;
            using (TextReader reader = new StringReader(str))
            {
                xmlTourn = (Tournament)xmlSerializer.Deserialize(reader);
            }

            var tourn =
                _context.Tournaments.Include(x => x.Sport)
                    .Include(x => x.Games)
                    .Include(x => x.TournamentParticipants)
                    .Single(x => x.Id == xmlTourn.Id);

            if (!xmlTourn.IsFriendlyTournament)
            {
                foreach (var tournamentParticipant in xmlTourn.TournamentParticipants.Where(x => x.Id == 0))
                {
                    tourn.TournamentParticipants.Add(tournamentParticipant);
                }
            }

            // игры
            foreach (var game in xmlTourn.Games)
            {
                if (game.WinnerId == 0) game.WinnerId = null;
                foreach (var gameEvent in game.Events)
                {
                    if (gameEvent.EventType.IsSystemEventType)
                    {
                        gameEvent.Count = 1;
                        gameEvent.GameParticipantPlayerId = null;
                        gameEvent.GameParticipantPlayer = null;
                        gameEvent.TimePeriod = null;
                        gameEvent.Time = null;
                    }
                    if (!gameEvent.EventType.IsSignifForProtocol)
                    {
                        gameEvent.TimePeriod = null;
                        gameEvent.Time = null;
                    }
                }

                if (game.Id == 0)
                {
                    tourn.Games.Add(game);
                    var events = new Event[game.Events.Count];
                    game.Events.CopyTo(events);
                    var oldEvents = events.ToList();

                    game.Events = new List<Event>();
                    _context.Entry(tourn).State = EntityState.Modified;
                    _context.SaveChanges();
                    foreach (var gameEvent in oldEvents)
                    {
                        if (gameEvent.GameParticipantPlayerId == null)
                        {
                            game.Events.Add(gameEvent);
                        }
                        else
                        {
                            var gamePlayers = game.GameParticipants.SelectMany(x => x.GameParticipantPlayers).ToList();
                            var player = gamePlayers.Single(x => x.PlayerId == gameEvent.GameParticipantPlayerId);
                            gameEvent.GameParticipantPlayer = player;
                            gameEvent.GameParticipantPlayerId = player.Id;
                            game.Events.Add(gameEvent);
                        }
                    }
                    _context.Entry(game).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                else
                {
                    var targetGame = tourn.Games.SingleOrDefault(x => x.Id == game.Id);
                    if (targetGame != null)
                    {
                        targetGame.Events = game.Events;
                        targetGame.WinnerId = game.WinnerId;

                        foreach (var gameEvent in game.Events)
                        {
                            if (gameEvent.GameParticipantPlayer == null)
                            {
                                game.Events.Add(gameEvent);
                            }
                            else
                            {
                                var gamePlayers = game.GameParticipants.SelectMany(x => x.GameParticipantPlayers).ToList();
                                var player = gamePlayers.Single(x => x.PlayerId == gameEvent.GameParticipantPlayerId);
                                gameEvent.GameParticipantPlayer = player;
                                gameEvent.GameParticipantPlayerId = player.Id;
                            }
                        }
                        _context.Entry(targetGame).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                }
            }
            _context.Entry(tourn).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public byte[] SerializeAnObject(object anObject)
        {
            var xmlSerializer = new XmlSerializer(anObject.GetType());
            var writer = new StringWriter();

            xmlSerializer.Serialize(writer, anObject);
            return GetBytes(writer.ToString());
        }

        private byte[] GetBytes(string str)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetBytes(str);
        }
    }
}
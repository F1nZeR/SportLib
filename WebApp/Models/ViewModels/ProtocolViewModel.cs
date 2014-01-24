using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Data;

namespace WebApp.Models.ViewModels
{
    public class ProtocolViewModel
    {
        public Sport Sport { get; set; }
        public List<EventType> EventTypes { get; set; }
        public List<GameParticipant> Participants { get; set; }
        public List<Event> Events { get; set; }
        
        public ProtocolViewModel(DataContext context, int gameId)
        {
            var game = context.Games.Single(x => x.Id == gameId);
            Events = game.Events.ToList();
            Participants = game.GameParticipants.ToList();
            Sport = game.Sport;
            EventTypes = game.Sport.EventTypes.Where(x => !x.IsSystemEventType).OrderBy(x => x.Id).ToList();
        }
    }
}
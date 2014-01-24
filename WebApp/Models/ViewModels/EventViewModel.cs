using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.ViewModels
{
    public class EventViewModel
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public string EventText { get; set; }
        public string Comment { get; set; }
        public int Count { get; set; }
        public Tuple<int, string> Player { get; set; }

        public EventViewModel(Event ev)
        {
            Id = ev.Id;
            Count = ev.Count;
            if (ev.Time.HasValue)
            {
                if (ev.Time.Value.Hours == 0)
                {
                    Time = string.Format("[{1} {2}] {0}", ev.Time.Value.ToString(@"mm\:ss"),
                        ev.TimePeriod, ev.EventType.Sport.TimePeriodName);
                }
                else
                {
                    Time = string.Format("[{1} {2}] {0}", ev.Time.Value.ToString(@"hh\:mm\:ss"),
                        ev.TimePeriod, ev.EventType.Sport.TimePeriodName);
                }
            }
            else
            {
                Time = null;
            }
            EventText = ev.EventType.Name;
            Comment = ev.Comment;
            Player = ev.GameParticipantPlayer != null
                ? Tuple.Create(ev.GameParticipantPlayer.Id, ev.GameParticipantPlayer.Player.Name)
                : null;
        }
    }
}
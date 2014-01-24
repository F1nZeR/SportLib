using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class GameParticipant
    {
        public int Id { get; set; }
        public int ParticipantId { get; set; }
        public int GameId { get; set; }

        public virtual Game Game { get; set; }
        public virtual Participant Participant { get; set; }
        public virtual List<GameParticipantPlayer> GameParticipantPlayers { get; set; }
    }
}
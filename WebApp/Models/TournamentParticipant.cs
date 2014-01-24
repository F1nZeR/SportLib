using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class TournamentParticipant
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public int ParticipantId { get; set; }

        public virtual Tournament Tournament { get; set; }
        public virtual Participant Participant { get; set; }
    }
}
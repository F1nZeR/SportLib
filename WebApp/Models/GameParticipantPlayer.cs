using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class GameParticipantPlayer
    {
        public int Id { get; set; }
        public int GameParticipantId { get; set; }
        public int PlayerId { get; set; }

        public virtual List<GameParticipantPlayerProp> GameParticipantPlayerProps { get; set; }
        public virtual GameParticipant GameParticipant { get; set; }
        public virtual Player Player { get; set; }
    }
}
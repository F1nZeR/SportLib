using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class GameParticipantPlayerProp
    {
        public int Id { get; set; }
        public int GameParticipantPlayerId { get; set; }
        public int PlayerPropertyTypeId { get; set; }
        public string PropValue { get; set; }

        public virtual GameParticipantPlayer GameParticipantPlayer { get; set; }
        public virtual PlayerPropertyType PlayerPropertyType { get; set; }
    }
}
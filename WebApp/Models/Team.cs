using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class Team : Participant
    {
        [Display(Name = "Город")]
        public string City { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set;}

        //[Display(Name = "Капитан")]
        //public int CaptainId { get; set; }
        //public Player Captain { get; set; }

        public virtual List<Player> Players { get; set; }
        public virtual List<TeamProperty> Properties { get; set; }
    }
}
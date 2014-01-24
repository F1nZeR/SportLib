using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Models
{
    public class Game
    {
        [HiddenInput]
        public int Id { get; set; }

        [Display(Name = "Дата и время")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Display(Name = "Место проведения")]
        public string Place { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Турнир")]
        public int TournamentId { get; set; }

        [Display(Name = "Победитель")]
        public int? WinnerId { get; set; }
        public int SportId { get; set; }

        public virtual List<GameParticipant> GameParticipants { get; set; }
        public virtual Participant Winner { get; set; }
        public virtual Sport Sport { get; set; }
        public virtual List<Event> Events { get; set; } 
        public virtual Tournament Tournament { get; set; }
    }
}
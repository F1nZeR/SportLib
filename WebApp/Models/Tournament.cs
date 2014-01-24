using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Models
{
    public class Tournament
    {
        [HiddenInput]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Дата начала")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Дата окончания")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Место проведения")]
        public string Place { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Ссылка на изображение")]
        public string ImageUrl { get; set; }

        [Display(Name = "Вид спорта")]
        public int SportId { get; set; }

        public bool IsFriendlyTournament { get; set; }

        public virtual Sport Sport { get; set; }
        public virtual List<TournamentParticipant> TournamentParticipants { get; set; }
        public virtual List<Game> Games { get; set; }
    }
}
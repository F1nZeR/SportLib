using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WebApp.Models
{
    public class Sport
    {
        [HiddenInput]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Название спорта")]
        public string Name { get; set; }

        [Display(Name = "Описание спорта")]
        public string Description { get; set; }

        [Display(Name = "Ссылка на изображение")]
        public string ImageUrl { get; set; }

        [Required]
        [Display(Name = "Название")]
        public string TimePeriodName { get; set; }

        [Required]
        [Display(Name = "Количество")]
        public int TimePeriodCount { get; set; }

        [Required]
        [Display(Name = "Сторон от")]
        public int SidesCountMin { get; set; }

        [Required]
        [Display(Name = "Сторон до")]
        public int SidesCountMax { get; set; }

        [Required]
        [Display(Name = "Командный вид спорта")]
        public bool IsTeamSport { get; set; }

        [Display(Name = "Участников от")]
        public int? TeamSizeMin { get; set; }
        [Display(Name = "Участников до")]
        public int? TeamSizeMax { get; set; }

        public virtual List<EventType> EventTypes { get; set; }
        public virtual List<PlayerPropertyType> PlayerPropertyTypes { get; set; }
        public virtual List<TeamPropertyType> TeamPropertyTypes { get; set; }
        public virtual List<Tournament> Tournaments { get; set; }
        public virtual List<Game> Games { get; set; }
        public List<Participant> Participants { get; set; }
    }
}
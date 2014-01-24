using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class Player : Participant
    {
        [Required(ErrorMessage = "Поле не может быть пустым")]
        [Display(Name = "Дата рождения игрока")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Поле не может быть пустым")]
        [Display(Name = "Пол игрока")]
        public Sex Sex { get; set; }

        [Display(Name = "Национальность игрока")]
        public string Nationality { get; set; }

        [Display(Name = "Биография игрока")]
        public string Biography { get; set; }

        public virtual List<PlayerProperty> Properties { get; set; }
        public virtual List<Team> Teams { get; set; }
    }

    public enum Sex
    {
        Male,
        Female
    }
}
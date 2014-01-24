using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Models
{
    public class Participant
    {
        [Required(ErrorMessage = "Поле не может быть пустым")]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [HiddenInput]
        public int SportId { get; set; }

        [Display(Name = "Ссылка на изображение")]
        public string ImageUrl { get; set; }

        [ScaffoldColumn(false)]
        public virtual Sport Sport { get; set; }
    }
}
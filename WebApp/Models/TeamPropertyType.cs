using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class TeamPropertyType
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Тип")]
        public AttributeTypeEnum Type { get; set; }

        public int SportId { get; set; }

        public virtual Sport Sport { get; set; }
    }
}
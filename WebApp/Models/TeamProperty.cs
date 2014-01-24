using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class TeamProperty
    {
        public int Id { get; set; }
        [Display(Name = "Значение")]
        public string Value { get; set; }

        public int TeamPropertyTypeId { get; set; }

        public virtual TeamPropertyType TeamPropertyType { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; }
    }
}
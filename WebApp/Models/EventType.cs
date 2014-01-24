using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Models
{
    public class EventType
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Значимое для протокола")]
        public bool IsSignifForProtocol { get; set; }
        [Display(Name = "Изменение счёта")]
        public int TotalChange { get; set; }
        [HiddenInput]
        public bool IsSystemEventType { get; set; }

        public int SportId { get; set; }

        public virtual Sport Sport { get; set; }
    }
}
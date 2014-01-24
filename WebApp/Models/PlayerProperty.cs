using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class PlayerProperty
    {
        public int Id { get; set; }
        [Display(Name = "Значение")]
        public string Value { get; set; }
        
        public int PlayerPropertyTypeId { get; set; }

        public virtual PlayerPropertyType PlayerPropertyType { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; }
    }
}
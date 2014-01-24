using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WebApp.Models
{
    public class Event
    {
        public int Id { get; set; }
        [Display(Name = "Количество")]
        public int Count { get; set; }
        [Display(Name = "Комментарий")]
        public string Comment { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:mm\\:ss}")]
        [Display(Name = "Тайм/период")]
        public int? TimePeriod { get; set; }
        [XmlIgnore]
        [Display(Name = "Время")]
        public TimeSpan? Time { get; set; }

        [XmlElement("Time")]
        [NotMapped]
        public string XmlTime
        {
            get { return Time.ToString(); }
            set { Time = TimeSpan.Parse(value); }
        }

        [Display(Name = "Участник события")]
        public int? GameParticipantPlayerId { get; set; }
        [Display(Name = "Тип события")]
        public int EventTypeId { get; set; }
        public int GameId { get; set; }

        public virtual Game Game { get; set; }
        public virtual EventType EventType { get; set; }
        public virtual GameParticipantPlayer GameParticipantPlayer { get; set; }
    }
}
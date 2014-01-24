using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.Statistic
{
    public class StatisticRequest
    {
        public int SportId { get; set; }
        public int[] TargetEvents { get; set; }
        public int SortEvent { get; set; }
        public int SortOrder { get; set; }
        public StatAgregateType AgregateType { get; set; }
        public StatTargetType StatType { get; set; }
        public int[] StatElements { get; set; }
        public StatTargetType ContextType { get; set; }
        public int[] ContextElems { get; set; }
    }
}
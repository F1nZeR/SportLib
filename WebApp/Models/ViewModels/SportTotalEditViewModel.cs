using System.Collections.Generic;

namespace WebApp.Models.ViewModels
{
    public class SportTotalEditViewModel
    {
        public Sport Sport { get; set; }
        public List<PlayerPropertyType> PlayerPropertyTypes { get; set; }
        public List<TeamPropertyType> TeamPropertyTypes { get; set; }
        public List<EventType> EventTypes { get; set; }

        public SportTotalEditViewModel()
        {
            PlayerPropertyTypes = new List<PlayerPropertyType>();
            TeamPropertyTypes = new List<TeamPropertyType>();
            EventTypes = new List<EventType>();
        }
    }
}
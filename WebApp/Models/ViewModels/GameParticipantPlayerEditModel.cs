using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApp.Models.ViewModels
{
    public class GameParticipantPlayerEditModel
    {
        public int Id { get; set; }

        [Display(Name = "Имя игрока")]
        public string PlayerName { get; set; }
        public List<GameParticipantPlayerProp> GameParticipantPlayerProps { get; set; }

        public static GameParticipantPlayerEditModel CreateFromModel(GameParticipantPlayer model)
        {
            return new GameParticipantPlayerEditModel
            {
                Id = model.Id,
                PlayerName = model.Player.Name,
                GameParticipantPlayerProps = model.GameParticipantPlayerProps.ToList()
            };
        }
    }
}
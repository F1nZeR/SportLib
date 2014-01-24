using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.ViewModels
{
    public class GamePlayerViewModel
    {
        public string Name { get; set; }
        public List<GameParticipantPlayerProp> GameParticipantPlayerProps { get; set; }
        public int Id { get; set; }
        public string ImageUrl { get; set; }

        public static List<GamePlayerViewModel> GetModels(List<GameParticipant> participants, List<Player> players)
        {
            var playersModels = new List<GamePlayerViewModel>();
            foreach (var player in players)
            {
                var model = new GamePlayerViewModel
                {
                    Id = participants.Single(x => x.ParticipantId == player.Id).GameParticipantPlayers.First().Id,
                    ImageUrl = player.ImageUrl,
                    Name = player.Name
                };
                var props =
                    participants.Single(x => x.ParticipantId == player.Id)
                        .GameParticipantPlayers.First()
                        .GameParticipantPlayerProps;
                model.GameParticipantPlayerProps = props != null ? props.ToList() : new List<GameParticipantPlayerProp>();
                playersModels.Add(model);
            }
            return playersModels;
        }
    }
}
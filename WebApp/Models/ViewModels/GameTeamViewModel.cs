using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Data;

namespace WebApp.Models.ViewModels
{
    public class GameTeamViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public List<GamePlayerViewModel> GamePlayerViewModels { get; set; }

        public static List<GameTeamViewModel> GetModels(List<GameParticipant> participants, List<Team> teams, DataContext context)
        {
            var res = new List<GameTeamViewModel>();
            foreach (var team in teams)
            {
                var participant = participants.Single(x => x.ParticipantId == team.Id);
                var resModel = new GameTeamViewModel
                {
                    Id = participant.Id,
                    Name = team.Name,
                    ImageUrl = team.ImageUrl,
                    GamePlayerViewModels = new List<GamePlayerViewModel>()
                };
                var players = participant.GameParticipantPlayers.ToList();
                foreach (var gameParticipantPlayer in players)
                {
                    var props = gameParticipantPlayer.GameParticipantPlayerProps;
                    resModel.GamePlayerViewModels.Add(new GamePlayerViewModel
                    {
                        Id = gameParticipantPlayer.Id,
                        Name = gameParticipantPlayer.Player.Name,
                        ImageUrl = gameParticipantPlayer.Player.ImageUrl,
                        GameParticipantPlayerProps = props == null ? new List<GameParticipantPlayerProp>() : props.ToList()
                    });
                }
                res.Add(resModel);
            }
            return res;
        }
    }
}
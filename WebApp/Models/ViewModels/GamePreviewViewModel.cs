using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApp.Data;

namespace WebApp.Models.ViewModels
{
    public class GamePreviewViewModel
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string RawDisplayName { get; set; }
        public string DateString { get; set; }
        public string GameResultString { get; set; }
        public Game Game { get; set; }
        
        public GamePreviewViewModel(Game game, DataContext context)
        {
            Id = game.Id;
            game = context.Games.Include(x => x.GameParticipants).Include(x => x.Events).Single(x => x.Id == Id);
            Game = game;
            var gamePartics = game.GameParticipants.ToList();
            var partics = gamePartics.Select(x => x.Participant).ToList();
            if (!gamePartics.Any())
            {
                DisplayName = "[Добавьте противников]";
                RawDisplayName = DisplayName;
            }
            else
            {
                DisplayName = string.Join(" - ", partics.Select(x => x.Name));
                if (partics.All(x => !string.IsNullOrEmpty(x.ImageUrl)))
                {
                    RawDisplayName = string.Join(" - ",
                        partics.Select(
                            x => string.Format("<img src='{0}' width='30' height='30' /> {1}", x.ImageUrl, x.Name)));
                }
                else
                {
                    RawDisplayName = DisplayName;
                }
            }
            DateString = game.Date.ToShortDateString();
            DisplayName += string.Format(" [{0}]", DateString);

            if (gamePartics.Any())
            {
                var results =
                    gamePartics.Select(
                        participant => participant.GameParticipantPlayers.Select(x => x.PlayerId).ToList())
                        .Select(players => game.Events.Where(x => !x.EventType.IsSystemEventType &&
                            players.Contains(x.GameParticipantPlayer.PlayerId) && x.EventType.TotalChange > 0)
                            .Sum(x => x.EventType.TotalChange*x.Count)).ToList();
                GameResultString = string.Join(" - ", results);
            }
        }
    }
}
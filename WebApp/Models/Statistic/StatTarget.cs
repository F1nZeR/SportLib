using System.Collections.Generic;
using System.ComponentModel;

namespace WebApp.Models.Statistic
{
    public enum StatTargetType
    {
        [Description("Игроки")]
        Players,
        [Description("Команды")]
        Teams,
        [Description("Турниры")]
        Tournaments,
        [Description("Игры")]
        Games
    }

    public class StatTarget
    {
        public int SportId { get; set; }
        public StatTargetType StatTargetType { get; set; }
        
        private StatTarget(int sportId, StatTargetType statTargetType)
        {
            SportId = sportId;
            StatTargetType = statTargetType;
        }

        public StatTarget() {}

        public static List<StatTarget> GetStatTargets(Sport sport)
        {
            var targets = new List<StatTarget> {new StatTarget(sport.Id, StatTargetType.Players)};
            if (sport.IsTeamSport)
            {
                targets.Add(new StatTarget(sport.Id, StatTargetType.Teams));                
            }
            targets.Add(new StatTarget(sport.Id, StatTargetType.Games));

            return targets;
        }
    }
}
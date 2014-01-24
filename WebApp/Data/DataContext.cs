using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using WebApp.Migrations;
using WebApp.Models;

namespace WebApp.Data
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext() : base("DefaultConnection") { }

        static DataContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, Configuration>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Player>().HasMany(x => x.Properties).WithRequired().WillCascadeOnDelete(false);
            //modelBuilder.Entity<PlayerProperty>().HasRequired(x => x.Player).WithMany(x=>x.Properties).WillCascadeOnDelete(false);
        }
        public DbSet<Config> Configs { get; set; }
        public DbSet<ChatMessageModel> ChatMessages { get; set; }
        public DbSet<TurnModel> Turns { get; set; }

        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Sport> Sports { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<PlayerPropertyType> PlayerPropertyTypes { get; set; }
        public DbSet<PlayerProperty> PlayerProperties { get; set; }
        public DbSet<TeamPropertyType> TeamPropertyTypes { get; set; }
        public DbSet<TeamProperty> TeamProperties { get; set; }

        public DbSet<Game> Games { get; set; }
        public DbSet<GameParticipant> GameParticipants { get; set; }
        public DbSet<GameParticipantPlayer> GameParticipantPlayers { get; set; }
        public DbSet<GameParticipantPlayerProp> GameParticipantPlayerProps { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentParticipant> TournamentParticipants { get; set; }
    }
}
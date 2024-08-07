﻿using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Services.Profile;
using Microsoft.VisualStudio.Services.Users;
using P02_FootballBetting.Data.Models;
using System.Drawing;
using System.Numerics;

namespace P02_FootballBetting.Data
{
    public class FootballBettingContext : DbContext
    {
        private const string ConnectionString = "Server=﻿DESKTOP-DOQUI53\\SQLEXPRESS;Database=FootballBetting;Integrated Security=True";

        public FootballBettingContext(DbContextOptions options)
            : base(options) { }


        public DbSet<Country> Countries { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<Bet> Bets { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<PlayerStatistic> PlayersStatistics { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(ConnectionString);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerStatistic>()
                .HasKey(ps => new { ps.PlayerId, ps.GameId });
        }
    }
}
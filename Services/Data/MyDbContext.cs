using Server.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data
{
    internal class MyDbContext : DbContext
    {


        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatType> ChatType { get; set; }
        public DbSet<Chat_User> Chats_Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Project_Team> Projects_Teams { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Team_User> Teams_Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project_Team>().HasNoKey();
            modelBuilder.Entity<Team_User>().HasNoKey();
            modelBuilder.Entity<Chat_User>().HasKey(cu => new { cu.ID_Chat, cu.ID_User});
            modelBuilder.Entity<Team_User>().HasKey(tu => new { tu.ID_Team, tu.ID_User});

        }





        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-E86S7QI;Database=TeamTaskDB;Trusted_Connection=True;TrustServerCertificate=true;MultipleActiveResultSets=true;");
            }
            base.OnConfiguring(optionsBuilder);
        }

    }
}

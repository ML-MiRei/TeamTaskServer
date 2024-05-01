using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreatDatabase.Data.Model;
using System.Net.Http.Headers;

namespace GreatDatabase.Data
{
    public class MyDbContext : DbContext
    {


        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatType> ChatType { get; set; }
        public DbSet<ProjectTaskStatus> ProjectTaskStatuses { get; set; }
        public DbSet<Chat_User> Chats_Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Project_User> Projects_Users { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Team_User> Teams_Users { get; set; }
        public DbSet<Sprint> Sprints { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Notification_User> Notifications_Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-E86S7QI;Database=TeamTaskDB;Trusted_Connection=True;TrustServerCertificate=true;MultipleActiveResultSets=true;");
                optionsBuilder.EnableSensitiveDataLogging();
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project_User>().HasKey(cu => new { cu.UserId, cu.ProjectId});
            modelBuilder.Entity<Chat_User>().HasKey(cu => new { cu.ChatId, cu.UserId});
            modelBuilder.Entity<Team_User>().HasKey(tu => new { tu.TeamId, tu.UserId});
            modelBuilder.Entity<Notification_User>().HasKey(tu => new { tu.NotificationId, tu.UserId});
        }

        static MyDbContext dbContext = new MyDbContext();
        public static MyDbContext GetInstance => dbContext;

    }
}

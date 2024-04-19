using AuthorizationServer.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationServer.Data.DataContext
{
    public class MsSqlContext : DbContext
    {
        public DbSet<LoginData> LoginData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-E86S7QI;Database=AuthorizationDB;Trusted_Connection=True;TrustServerCertificate=true;MultipleActiveResultSets=true;");
            }
            base.OnConfiguring(optionsBuilder);
        }

    }
}

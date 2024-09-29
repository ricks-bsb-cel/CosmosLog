using CosmosLog.EntityFramework.Model;
using Microsoft.EntityFrameworkCore;

namespace CosmosLog.EntityFramework
{
    public class CosmosDbContext : DbContext
    {
        public CosmosDbContext(DbContextOptions<CosmosDbContext> options) : base(options) { }

        public DbSet<CosmosLogModel> CosmosLog { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CosmosLogModel>().HasKey(e => e.Id);
        }
    }
}

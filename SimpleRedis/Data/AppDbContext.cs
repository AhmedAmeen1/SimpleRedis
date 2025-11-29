using Microsoft.EntityFrameworkCore;
using SimpleRedis.Models;

namespace SimpleRedis.Data
{
    public class AppDbContext : DbContext
    {

        public DbSet<Order> Orders { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Order>().HasKey(o => o.Id);
            modelBuilder.Entity<Order>().Property(o => o.CustomerName).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Order>().Property(o => o.Product).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Order>().Property(o => o.Amount).IsRequired().HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Order>().Property(o => o.CreatedDate).IsRequired();
        }
    }
}

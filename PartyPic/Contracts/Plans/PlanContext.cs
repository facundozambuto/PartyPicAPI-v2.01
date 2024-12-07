using Microsoft.EntityFrameworkCore;
using PartyPic.Models.Plans;

namespace PartyPic.Contracts.Plans
{
    public class PlanContext : DbContext
    {
        public DbSet<Plan> Plans { get; set; }
        public DbSet<PriceHistory> PriceHistories { get; set; }

        public PlanContext(DbContextOptions<PlanContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PriceHistory>()
                .ToTable("PriceHistory");

            modelBuilder.Entity<Plan>()
                .Property(p => p.CreatedDatetime)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<PriceHistory>()
                .Property(ph => ph.CreatedDatetime)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Plan>()
                .HasMany(p => p.PriceHistories)
                .WithOne(ph => ph.Plan)
                .HasForeignKey(ph => ph.PlanId);

            modelBuilder.Entity<PriceHistory>()
                .Property(ph => ph.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}

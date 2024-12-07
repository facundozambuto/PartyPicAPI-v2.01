using Microsoft.EntityFrameworkCore;
using PartyPic.Models.Plans;
using PartyPic.Models.Subscriptions;

namespace PartyPic.Contracts.Subscriptions
{
    public class SubscriptionContext : DbContext
    {
        public SubscriptionContext(DbContextOptions<SubscriptionContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscription>()
            .ToTable("Subscriptions")
            .HasOne(s => s.User)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Subscription>()
                .Property(s => s.CreatedDatetime)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Plan)
                .WithMany()
                .HasForeignKey(s => s.PlanId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Subscription>()
                .Property(s => s.IsAutoRenew)
                .HasDefaultValue(false);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Plan> Plans { get; set; }
    }
}

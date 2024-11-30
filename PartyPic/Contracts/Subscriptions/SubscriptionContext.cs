using Microsoft.EntityFrameworkCore;
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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(s => s.SubscriptionId);

                entity.Property(s => s.PlanType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(s => s.MercadoPagoId)
                    .HasMaxLength(100);

                entity.Property(s => s.CreatedDatetime)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(s => s.User)
                    .WithMany(u => u.Subscriptions)
                    .HasForeignKey(s => s.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public DbSet<Subscription> Subscriptions { get; set; }
    }
}

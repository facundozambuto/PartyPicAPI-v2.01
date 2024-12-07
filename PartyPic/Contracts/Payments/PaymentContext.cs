namespace PartyPic.Contracts.Payments
{
    using Microsoft.EntityFrameworkCore;
    using PartyPic.Models.Payments;

    public class PaymentContext : DbContext
    {
        public PaymentContext(DbContextOptions<PaymentContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payments");

                entity.HasKey(p => p.PaymentId);

                entity.Property(p => p.PaymentType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(p => p.PaymentStatus)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(p => p.MercadoPagoId)
                    .HasMaxLength(100);

                entity.Property(p => p.CreatedDatetime)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(p => p.User)
                    .WithMany(u => u.Payments)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(p => p.Amount)
                    .HasPrecision(18, 2);

                entity.HasOne(p => p.Event)
                    .WithMany(e => e.Payments)
                    .HasForeignKey(p => p.EventId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }

        public DbSet<Payment> Payments { get; set; }
    }
}

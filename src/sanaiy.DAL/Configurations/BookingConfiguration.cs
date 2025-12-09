using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using sanaiy.BLL.Entities;
using sanaiy.BLL.Enums;

namespace sanaiy.DAL.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            // Table Name
            builder.ToTable("Bookings");

            // Primary Key
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(b => b.ServicesName)
                .HasMaxLength(255);

            builder.Property(b => b.AdditionalNote)
                .HasMaxLength(1000);

            builder.Property(b => b.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(b => b.PreferredDate)
                .IsRequired(false);

            builder.Property(b => b.PreferredTime)
                .HasMaxLength(20);

            builder.Property(b => b.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(BookingStatus.Pending);

            builder.Property(b => b.PaymentMethod)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(b => b.PriceFinal)
                .HasPrecision(18, 2);

            builder.Property(b => b.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(b => b.UpdatedAt)
                .IsRequired(false);

            // Foreign Keys
            builder.Property(b => b.ClientId)
                .IsRequired();

            builder.Property(b => b.CraftsmanId)
                .IsRequired(false);

            builder.Property(b => b.ServiceId)
                .IsRequired();

            builder.Property(b => b.LocationId)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(b => b.ClientId)
                .HasDatabaseName("IX_Bookings_ClientId");

            builder.HasIndex(b => b.CraftsmanId)
                .HasDatabaseName("IX_Bookings_CraftsmanId");

            builder.HasIndex(b => b.ServiceId)
                .HasDatabaseName("IX_Bookings_ServiceId");

            builder.HasIndex(b => b.Status)
                .HasDatabaseName("IX_Bookings_Status");

            builder.HasIndex(b => b.CreatedAt)
                .HasDatabaseName("IX_Bookings_CreatedAt");

            builder.HasIndex(b => new { b.ClientId, b.Status })
                .HasDatabaseName("IX_Bookings_ClientId_Status");

            builder.HasIndex(b => new { b.CraftsmanId, b.Status })
                .HasDatabaseName("IX_Bookings_CraftsmanId_Status");

            // Relationships
            builder.HasMany(b => b.Quotes)
                .WithOne(q => q.Booking)
                .HasForeignKey(q => q.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.Review)
                .WithOne(r => r.Booking)
                .HasForeignKey<Review>(r => r.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.PaymentTransaction)
                .WithOne(p => p.Booking)
                .HasForeignKey<PaymentTransaction>(p => p.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.Violations)
                .WithOne(v => v.TargetBooking)
                .HasForeignKey(v => v.TargetBookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Commission)
                .WithOne(c => c.Booking)
                .HasForeignKey<Commission>(c => c.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.WalletTransactions)
                .WithOne(wt => wt.RelatedBooking)
                .HasForeignKey(wt => wt.RelatedBookingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
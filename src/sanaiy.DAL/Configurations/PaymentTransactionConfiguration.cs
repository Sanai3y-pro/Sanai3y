using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using sanaiy.BLL.Entities;
using sanaiy.BLL.Enums;

namespace sanaiy.DAL.Configurations
{
    public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            // Table Name
            builder.ToTable("PaymentTransactions");

            // Primary Key
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(p => p.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(p => p.Method)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(PaymentStatus.Pending);

            builder.Property(p => p.Date)
                .IsRequired();

            builder.Property(p => p.ProviderReference)
                .HasMaxLength(255);

            builder.Property(p => p.ProviderName)
                .HasMaxLength(100);

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.UpdatedAt)
                .IsRequired(false);

            // Foreign Key (1:1 with Booking)
            builder.Property(p => p.BookingId)
                .IsRequired();

            // Indexes
            builder.HasIndex(p => p.BookingId)
                .IsUnique()
                .HasDatabaseName("IX_PaymentTransactions_BookingId");

            builder.HasIndex(p => p.Status)
                .HasDatabaseName("IX_PaymentTransactions_Status");

            builder.HasIndex(p => p.ProviderReference)
                .HasDatabaseName("IX_PaymentTransactions_ProviderReference");

            builder.HasIndex(p => p.Date)
                .HasDatabaseName("IX_PaymentTransactions_Date");

            // Relationships (already defined in Booking config)
        }
    }
}
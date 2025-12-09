using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using sanaiy.BLL.Entities;
using sanaiy.BLL.Enums;

namespace sanaiy.DAL.Configurations
{
    public class CommissionConfiguration : IEntityTypeConfiguration<Commission>
    {
        public void Configure(EntityTypeBuilder<Commission> builder)
        {
            // Table Name
            builder.ToTable("Commissions");

            // Primary Key
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(c => c.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(CommissionStatus.Pending);

            builder.Property(c => c.ChargedAt)
                .IsRequired(false);

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(c => c.UpdatedAt)
                .IsRequired(false);

            // Foreign Keys (1:1 with Booking)
            builder.Property(c => c.BookingId)
                .IsRequired();

            builder.Property(c => c.CraftsmanId)
                .IsRequired();

            builder.Property(c => c.WalletTransactionId)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(c => c.BookingId)
                .IsUnique()
                .HasDatabaseName("IX_Commissions_BookingId");

            builder.HasIndex(c => c.CraftsmanId)
                .HasDatabaseName("IX_Commissions_CraftsmanId");

            builder.HasIndex(c => c.Status)
                .HasDatabaseName("IX_Commissions_Status");

            builder.HasIndex(c => c.WalletTransactionId)
                .IsUnique()
                .HasDatabaseName("IX_Commissions_WalletTransactionId");

            // Relationships
            builder.HasOne(c => c.WalletTransaction)
                .WithOne(wt => wt.Commission)
                .HasForeignKey<Commission>(c => c.WalletTransactionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
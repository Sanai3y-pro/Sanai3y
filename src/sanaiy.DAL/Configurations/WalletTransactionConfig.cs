using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using sanaiy.BLL.Entities;

namespace sanaiy.DAL.Configurations
{
    public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> builder)
        {
            // Table Name
            builder.ToTable("WalletTransactions");

            // Primary Key
            builder.HasKey(wt => wt.Id);
            builder.Property(wt => wt.Id)
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(wt => wt.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(wt => wt.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(wt => wt.Direction)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(10);

            builder.Property(wt => wt.BalanceAfter)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(wt => wt.Note)
                .HasMaxLength(500);

            builder.Property(wt => wt.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(wt => wt.UpdatedAt)
                .IsRequired(false);

            // Foreign Keys
            builder.Property(wt => wt.WalletId)
                .IsRequired();

            builder.Property(wt => wt.RelatedBookingId)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(wt => wt.WalletId)
                .HasDatabaseName("IX_WalletTransactions_WalletId");

            builder.HasIndex(wt => wt.RelatedBookingId)
                .HasDatabaseName("IX_WalletTransactions_RelatedBookingId");

            builder.HasIndex(wt => wt.Type)
                .HasDatabaseName("IX_WalletTransactions_Type");

            builder.HasIndex(wt => wt.CreatedAt)
                .HasDatabaseName("IX_WalletTransactions_CreatedAt");

            builder.HasIndex(wt => new { wt.WalletId, wt.CreatedAt })
                .HasDatabaseName("IX_WalletTransactions_WalletId_CreatedAt");

            // Relationships (already defined in Wallet and Booking configs)
        }
    }
}
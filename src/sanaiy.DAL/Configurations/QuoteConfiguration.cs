using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using sanaiy.BLL.Entities;
using sanaiy.BLL.Enums;

namespace sanaiy.DAL.Configurations
{
    public class QuoteConfiguration : IEntityTypeConfiguration<Quote>
    {
        public void Configure(EntityTypeBuilder<Quote> builder)
        {
            // Table Name
            builder.ToTable("Quotes");

            // Primary Key
            builder.HasKey(q => q.Id);
            builder.Property(q => q.Id)
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(q => q.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(q => q.Duration)
                .IsRequired(false);

            builder.Property(q => q.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(QuoteStatus.Sent);

            builder.Property(q => q.Note)
                .HasMaxLength(500);

            builder.Property(q => q.ExpiresAt)
                .IsRequired(false);

            builder.Property(q => q.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(q => q.UpdatedAt)
                .IsRequired(false);

            // Foreign Keys
            builder.Property(q => q.BookingId)
                .IsRequired();

            builder.Property(q => q.CraftsmanId)
                .IsRequired();

            // Indexes
            builder.HasIndex(q => q.BookingId)
                .HasDatabaseName("IX_Quotes_BookingId");

            builder.HasIndex(q => q.CraftsmanId)
                .HasDatabaseName("IX_Quotes_CraftsmanId");

            builder.HasIndex(q => q.Status)
                .HasDatabaseName("IX_Quotes_Status");

            builder.HasIndex(q => new { q.BookingId, q.CraftsmanId })
                .HasDatabaseName("IX_Quotes_BookingId_CraftsmanId");

            // Relationships (already defined in Booking and Craftsman configs)
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using sanaiy.BLL.Entities;

namespace sanaiy.DAL.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            // Table Name
            builder.ToTable("Reviews");

            // Primary Key
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id)
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(r => r.Rating)
                .IsRequired();

            builder.Property(r => r.Comment)
                .HasMaxLength(1000);

            builder.Property(r => r.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(r => r.UpdatedAt)
                .IsRequired(false);

            // Foreign Keys (1:1 with Booking)
            builder.Property(r => r.BookingId)
                .IsRequired();

            builder.Property(r => r.ClientId)
                .IsRequired();

            builder.Property(r => r.CraftsmanId)
                .IsRequired();

            // Check Constraint: Rating between 1 and 5
            builder.HasCheckConstraint(
                "CK_Review_Rating",
                "Rating >= 1 AND Rating <= 5"
            );

            // Indexes
            builder.HasIndex(r => r.BookingId)
                .IsUnique()
                .HasDatabaseName("IX_Reviews_BookingId");

            builder.HasIndex(r => r.ClientId)
                .HasDatabaseName("IX_Reviews_ClientId");

            builder.HasIndex(r => r.CraftsmanId)
                .HasDatabaseName("IX_Reviews_CraftsmanId");

            builder.HasIndex(r => r.Rating)
                .HasDatabaseName("IX_Reviews_Rating");

            builder.HasIndex(r => r.CreatedAt)
                .HasDatabaseName("IX_Reviews_CreatedAt");

            // Relationships (already defined in Booking, Client, and Craftsman configs)
        }
    }
}
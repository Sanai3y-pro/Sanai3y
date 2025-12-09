using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using sanaiy.BLL.Entities;

namespace sanaiy.DAL.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            // Table Name
            builder.ToTable("Services");

            // Primary Key
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(s => s.ServiceName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(s => s.Description)
                .HasMaxLength(1000);

            builder.Property(s => s.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(s => s.IsPriceFixed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(s => s.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(s => s.UpdatedAt)
                .IsRequired(false);

            // Foreign Keys
            builder.Property(s => s.CategoryId)
                .IsRequired();

            builder.Property(s => s.OwnerCraftsmanId)
                .IsRequired();

            // Indexes
            builder.HasIndex(s => s.CategoryId)
                .HasDatabaseName("IX_Services_CategoryId");

            builder.HasIndex(s => s.OwnerCraftsmanId)
                .HasDatabaseName("IX_Services_OwnerCraftsmanId");

            builder.HasIndex(s => s.IsActive)
                .HasDatabaseName("IX_Services_IsActive");

            builder.HasIndex(s => new { s.ServiceName, s.OwnerCraftsmanId })
                .HasDatabaseName("IX_Services_ServiceName_OwnerCraftsmanId");

            // Relationships (already defined in Category and Craftsman configs)
            builder.HasMany(s => s.Bookings)
                .WithOne(b => b.Service)
                .HasForeignKey(b => b.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
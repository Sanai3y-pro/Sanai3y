using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using sanaiy.BLL.Entities;

namespace sanaiy.DAL.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            // Table Name
            builder.ToTable("Addresses");

            // Primary Key
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(a => a.FullAddress)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.AddressType)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(a => a.City)
                .HasMaxLength(100);

            builder.Property(a => a.IsDefault)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(a => a.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(a => a.UpdatedAt)
                .IsRequired(false);

            // Foreign Keys (nullable - one must be set)
            builder.Property(a => a.ClientId)
                .IsRequired(false);

            builder.Property(a => a.CraftsmanId)
                .IsRequired(false);

            // Check Constraint: Either ClientId OR CraftsmanId must be set
            builder.HasCheckConstraint(
                "CK_Address_Owner",
                "(ClientId IS NOT NULL AND CraftsmanId IS NULL) OR (ClientId IS NULL AND CraftsmanId IS NOT NULL)"
            );

            // Indexes
            builder.HasIndex(a => a.ClientId)
                .HasDatabaseName("IX_Addresses_ClientId");

            builder.HasIndex(a => a.CraftsmanId)
                .IsUnique()
                .HasDatabaseName("IX_Addresses_CraftsmanId");

            builder.HasIndex(a => new { a.ClientId, a.IsDefault })
                .HasDatabaseName("IX_Addresses_ClientId_IsDefault");

            // Relationships
            builder.HasMany(a => a.Bookings)
                .WithOne(b => b.Location)
                .HasForeignKey(b => b.LocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using sanaiy.BLL.Entities;
using sanaiy.BLL.Enums;

namespace sanaiy.DAL.Configurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            // Table Name
            builder.ToTable("Clients");

            // Primary Key
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(c => c.Fname)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Lname)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(c => c.Password)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(c => c.Phone)
                .HasMaxLength(20);

            builder.Property(c => c.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(UserStatus.PendingEmailConfirmation);

            builder.Property(c => c.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(c => c.ProfileImageUrl)
                .HasMaxLength(500);

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(c => c.UpdatedAt)
                .IsRequired(false);

            // Computed Column (Ignored - handled in code)
            builder.Ignore(c => c.FullName);

            // Indexes
            builder.HasIndex(c => c.Email)
                .IsUnique()
                .HasDatabaseName("IX_Clients_Email");

            builder.HasIndex(c => c.Status)
                .HasDatabaseName("IX_Clients_Status");

            builder.HasIndex(c => c.IsActive)
                .HasDatabaseName("IX_Clients_IsActive");

            // Relationships
            builder.HasMany(c => c.Bookings)
                .WithOne(b => b.Client)
                .HasForeignKey(b => b.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Addresses)
                .WithOne(a => a.Client)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Reviews)
                .WithOne(r => r.Client)
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.PaymentTransactions)
                .WithOne()
                .HasForeignKey("ClientId")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
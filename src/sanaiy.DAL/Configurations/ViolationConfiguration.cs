using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using sanaiy.BLL.Entities;
using sanaiy.BLL.Enums;

namespace sanaiy.DAL.Configurations
{
    public class ViolationConfiguration : IEntityTypeConfiguration<Violation>
    {
        public void Configure(EntityTypeBuilder<Violation> builder)
        {
            // Table Name
            builder.ToTable("Violations");

            // Primary Key
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Id)
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(v => v.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(v => v.Description)
                .HasMaxLength(1000);

            builder.Property(v => v.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(ViolationStatus.Submitted);

            builder.Property(v => v.AdminNote)
                .HasMaxLength(500);

            builder.Property(v => v.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(v => v.UpdatedAt)
                .IsRequired(false);

            // Foreign Keys
            builder.Property(v => v.ReporterUserId)
                .IsRequired();

            builder.Property(v => v.TargetBookingId)
                .IsRequired(false);

            builder.Property(v => v.TargetUserId)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(v => v.ReporterUserId)
                .HasDatabaseName("IX_Violations_ReporterUserId");

            builder.HasIndex(v => v.TargetBookingId)
                .HasDatabaseName("IX_Violations_TargetBookingId");

            builder.HasIndex(v => v.TargetUserId)
                .HasDatabaseName("IX_Violations_TargetUserId");

            builder.HasIndex(v => v.Status)
                .HasDatabaseName("IX_Violations_Status");

            builder.HasIndex(v => v.CreatedAt)
                .HasDatabaseName("IX_Violations_CreatedAt");

            // Relationships (TargetBooking already defined in Booking config)
            // Note: ReporterUser and TargetUser need special handling in code
            // as they can be either Client or Craftsman
        }
    }
}
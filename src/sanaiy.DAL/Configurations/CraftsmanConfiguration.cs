using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using sanaiy.BLL.Entities;
using sanaiy.BLL.Enums;

namespace sanaiy.DAL.Configurations
{
    public class CraftsmanConfiguration : IEntityTypeConfiguration<Craftsman>
    {
        public void Configure(EntityTypeBuilder<Craftsman> builder)
        {
            // Table Name
            builder.ToTable("Craftsmen");

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

            builder.Property(c => c.NationalID)
                .HasMaxLength(20);

            builder.Property(c => c.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(CraftsmanApplicationStatus.Applied);

            builder.Property(c => c.IsVerified)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.Category)
                .HasMaxLength(100);

            builder.Property(c => c.YearsOfExperience)
                .IsRequired(false);

            builder.Property(c => c.Bio)
                .HasMaxLength(500);

            builder.Property(c => c.RatingAverage)
                .IsRequired()
                .HasPrecision(3, 2)
                .HasDefaultValue(0.00m);

            builder.Property(c => c.ProfileImageUrl)
                .HasMaxLength(500);

            builder.Property(c => c.IDCardImage)
                .HasMaxLength(500);

            builder.Property(c => c.DrugTestFile)
                .HasMaxLength(500);

            builder.Property(c => c.CriminalRecordFile)
                .HasMaxLength(500);

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(c => c.UpdatedAt)
                .IsRequired(false);

            // Computed Column (Ignored)
            builder.Ignore(c => c.FullName);

            // Indexes
            builder.HasIndex(c => c.Email)
                .IsUnique()
                .HasDatabaseName("IX_Craftsmen_Email");

            builder.HasIndex(c => c.Status)
                .HasDatabaseName("IX_Craftsmen_Status");

            builder.HasIndex(c => c.IsVerified)
                .HasDatabaseName("IX_Craftsmen_IsVerified");

            builder.HasIndex(c => c.Category)
                .HasDatabaseName("IX_Craftsmen_Category");

            // Relationships
            builder.HasMany(c => c.Services)
                .WithOne(s => s.OwnerCraftsman)
                .HasForeignKey(s => s.OwnerCraftsmanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Bookings)
                .WithOne(b => b.Craftsman)
                .HasForeignKey(b => b.CraftsmanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Quotes)
                .WithOne(q => q.Craftsman)
                .HasForeignKey(q => q.CraftsmanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.ReviewsReceived)
                .WithOne(r => r.Craftsman)
                .HasForeignKey(r => r.CraftsmanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Commissions)
                .WithOne(com => com.Craftsman)
                .HasForeignKey(com => com.CraftsmanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Wallet)
                .WithOne(w => w.Craftsman)
                .HasForeignKey<Wallet>(w => w.CraftsmanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Address)
                .WithOne(a => a.Craftsman)
                .HasForeignKey<Address>(a => a.CraftsmanId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
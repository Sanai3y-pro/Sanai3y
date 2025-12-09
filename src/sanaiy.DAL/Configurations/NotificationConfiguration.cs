using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using sanaiy.BLL.Entities;

namespace sanaiy.DAL.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            builder.HasKey(n => n.Id);
            builder.Property(n => n.Id).ValueGeneratedOnAdd();

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(n => n.Type)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Info");

            builder.Property(n => n.IsRead)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(n => n.Link)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(n => n.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(n => n.UpdatedAt)
                .IsRequired(false);

            // Foreign keys
            builder.Property(n => n.ClientId).IsRequired(false);
            builder.Property(n => n.CraftsmanId).IsRequired(false);

            builder.HasIndex(n => n.ClientId).HasDatabaseName("IX_Notifications_ClientId");
            builder.HasIndex(n => n.CraftsmanId).HasDatabaseName("IX_Notifications_CraftsmanId");

            // Relationships
            builder.HasOne(n => n.Client)
                .WithMany(); // client notifications - we don't require navigation back-collection in Client

            builder.HasOne(n => n.Craftsman)
                .WithMany();
        }
    }
}
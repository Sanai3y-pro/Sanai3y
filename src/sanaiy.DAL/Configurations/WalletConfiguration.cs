using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using sanaiy.BLL.Entities;
using System;

namespace sanaiy.DAL.Configurations
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.ToTable("Wallets");
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Balance).HasColumnType("decimal(18,2)").IsRequired().HasDefaultValue(0.00m);
            builder.Property(w => w.IsFrozen).HasDefaultValue(false);
            builder.Property(w => w.CreatedAt).IsRequired();

            // 1:1 relation: Wallet.CraftsmanId is FK and unique
            builder.HasOne(w => w.Craftsman)
                   .WithOne(c => c.Wallet)
                   .HasForeignKey<Wallet>(w => w.CraftsmanId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(w => w.CraftsmanId).IsUnique();

            // Optimistic concurrency: RowVersion as shadow property
            builder.Property<byte[]>("RowVersion").IsRowVersion().IsConcurrencyToken();
        }
    }
}
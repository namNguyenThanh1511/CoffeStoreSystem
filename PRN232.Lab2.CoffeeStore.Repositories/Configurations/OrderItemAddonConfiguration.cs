using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN232.Lab2.CoffeeStore.Repositories.Entities;

namespace PRN232.Lab2.CoffeeStore.Repositories.Configurations
{
    public class OrderItemAddonConfiguration : IEntityTypeConfiguration<OrderItemAddon>
    {
        public void Configure(EntityTypeBuilder<OrderItemAddon> builder)
        {
            builder.ToTable("OrderItemAddons");
            builder.HasKey(oia => oia.Id);
            builder.Property(oia => oia.Id).ValueGeneratedOnAdd();

            builder.Property(oia => oia.Price)
                .HasColumnType("decimal(10,2)")
                .IsRequired();
            // ✅ Quan hệ N–1 với OrderDetail (1 OrderDetail có nhiều OrderItemAddons)
            builder.HasOne(oia => oia.OrderDetail)
                   .WithMany(od => od.OrderItemAddons)
                   .HasForeignKey(oia => oia.OrderDetailId)
                   .OnDelete(DeleteBehavior.Cascade);

            // ✅ Quan hệ N–1 với CoffeeAddon
            builder.HasOne(oia => oia.Addon)
                   .WithMany()
                   .HasForeignKey(oia => oia.AddonId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}



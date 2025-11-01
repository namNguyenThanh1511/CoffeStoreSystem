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

            builder.HasOne(oia => oia.OrderDetail)
                .WithMany()
                .HasForeignKey(oia => oia.OrderDetailId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(oia => oia.Addon)
                .WithMany()
                .HasForeignKey(oia => oia.AddonId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}



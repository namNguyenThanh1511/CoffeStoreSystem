using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN232.Lab2.CoffeeStore.Repositories.Entities;

namespace PRN232.Lab2.CoffeeStore.Repositories.Configurations
{
    public class CoffeeVariantConfiguration : IEntityTypeConfiguration<CoffeeVariant>
    {
        public void Configure(EntityTypeBuilder<CoffeeVariant> builder)
        {
            builder.ToTable("CoffeeVariants");

            builder.HasKey(cv => cv.Id);
            builder.Property(cv => cv.Id).ValueGeneratedOnAdd();

            builder.Property(cv => cv.ProductId)
                .IsRequired();

            builder.Property(cv => cv.Size)
                .IsRequired();

            builder.Property(cv => cv.BasePrice)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(cv => cv.IsActive)
                .IsRequired();

            builder.Property(cv => cv.CreatedAt)
                .IsRequired();

            builder.HasOne(cv => cv.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(cv => cv.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(cv => cv.OrderDetails)
                .WithOne(od => od.Variant)
                .HasForeignKey(od => od.VariantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}



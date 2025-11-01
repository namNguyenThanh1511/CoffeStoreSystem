using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN232.Lab2.CoffeeStore.Repositories.Entities;

namespace PRN232.Lab2.CoffeeStore.Repositories.Configurations
{
    public class CoffeeAddonConfiguration : IEntityTypeConfiguration<CoffeeAddon>
    {
        public void Configure(EntityTypeBuilder<CoffeeAddon> builder)
        {
            builder.ToTable("CoffeeAddons");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.Price)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(a => a.IsActive).IsRequired();
            builder.Property(a => a.CreatedAt).IsRequired();
        }
    }
}



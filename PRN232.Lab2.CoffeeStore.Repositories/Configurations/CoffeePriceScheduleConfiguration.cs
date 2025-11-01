using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN232.Lab2.CoffeeStore.Repositories.Entities;

namespace PRN232.Lab2.CoffeeStore.Repositories.Configurations
{
    public class CoffeePriceScheduleConfiguration : IEntityTypeConfiguration<CoffeePriceSchedule>
    {
        public void Configure(EntityTypeBuilder<CoffeePriceSchedule> builder)
        {
            builder.ToTable("CoffeePriceSchedules");
            builder.HasKey(ps => ps.Id);
            builder.Property(ps => ps.Id).ValueGeneratedOnAdd();

            builder.Property(ps => ps.Price)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(ps => ps.DayMask).IsRequired();
            builder.Property(ps => ps.StartHour).IsRequired();
            builder.Property(ps => ps.EndHour).IsRequired();
            builder.Property(ps => ps.CreatedAt).IsRequired();

            builder.HasOne(ps => ps.Variant)
                .WithMany()
                .HasForeignKey(ps => ps.VariantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}



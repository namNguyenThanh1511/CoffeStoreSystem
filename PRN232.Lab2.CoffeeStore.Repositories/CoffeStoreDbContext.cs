using Microsoft.EntityFrameworkCore;
using PRN232.Lab2.CoffeeStore.Repositories.Configurations;
using PRN232.Lab2.CoffeeStore.Repositories.Entities;

namespace PRN232.Lab2.CoffeeStore.Repositories
{
    public class CoffeStoreDbContext : DbContext
    {
        public CoffeStoreDbContext(DbContextOptions<CoffeStoreDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new MenuConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new ProductInMenuConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderDetailConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new CoffeeVariantConfiguration());
            modelBuilder.ApplyConfiguration(new CoffeeAddonConfiguration());
            modelBuilder.ApplyConfiguration(new CoffeePriceScheduleConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemAddonConfiguration());
        }


        public DbSet<Product> Products { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<ProductInMenu> ProductInMenus { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<CoffeeVariant> CoffeeVariants { get; set; }
        public DbSet<CoffeeAddon> CoffeeAddons { get; set; }
        public DbSet<CoffeePriceSchedule> CoffeePriceSchedules { get; set; }
        public DbSet<OrderItemAddon> OrderItemAddons { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;
using PRN232.Lab2.CoffeeStore.Repositories;
using PRN232.Lab2.CoffeeStore.Repositories.Entities;

namespace PRN232.Lab2.CoffeeStore.API
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CoffeStoreDbContext>();
            var hasher = new PasswordHasher<User>();
            await SeedCoffeVariantsAsync(context);

        }

        private static async Task SeedCoffeVariantsAsync(CoffeStoreDbContext context)
        {
            // Seed Products with CoffeeVariants if empty
            if (!context.Products.Any())
            {
                var espresso = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Espresso",
                    Description = "Strong black coffee shot",
                    Origin = "Italy",
                    RoastLevel = RoastLevel.Medium,
                    BrewMethod = BrewMethod.Espresso,
                    ImageUrl = null,
                    IsActive = true,
                    Variants = new List<CoffeeVariant>
                    {
                        new CoffeeVariant { Id = Guid.NewGuid(), Size = CoffeeSize.S, BasePrice = 10000, IsActive = true },
                        new CoffeeVariant { Id = Guid.NewGuid(), Size = CoffeeSize.M, BasePrice = 20000, IsActive = true },
                        new CoffeeVariant { Id = Guid.NewGuid(), Size = CoffeeSize.L, BasePrice = 25000, IsActive = true },
                    }
                };

                var latte = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Latte",
                    Description = "Coffee with steamed milk",
                    Origin = "Blend",
                    RoastLevel = RoastLevel.Light,
                    BrewMethod = BrewMethod.Espresso,
                    ImageUrl = null,
                    IsActive = true,
                    Variants = new List<CoffeeVariant>
                    {
                        new CoffeeVariant { Id = Guid.NewGuid(), Size = CoffeeSize.S, BasePrice = 25000, IsActive = true },
                        new CoffeeVariant { Id = Guid.NewGuid(), Size = CoffeeSize.M, BasePrice = 29000, IsActive = true },
                        new CoffeeVariant { Id = Guid.NewGuid(), Size = CoffeeSize.L, BasePrice = 33000, IsActive = true },
                    }
                };

                await context.Products.AddRangeAsync(espresso, latte);
            }

            // Ensure each active product has at least one active variant
            var productsWithoutVariants = context.Products
                .Where(p => p.IsActive && !context.CoffeeVariants.Any(v => v.ProductId == p.Id && v.IsActive))
                .ToList();
            if (productsWithoutVariants.Any())
            {
                var newVariants = new List<CoffeeVariant>();
                foreach (var p in productsWithoutVariants)
                {
                    newVariants.Add(new CoffeeVariant { Id = Guid.NewGuid(), ProductId = p.Id, Size = CoffeeSize.M, BasePrice = 20000, IsActive = true });
                }
                await context.CoffeeVariants.AddRangeAsync(newVariants);
            }

            await context.SaveChangesAsync();
        }
    }
}

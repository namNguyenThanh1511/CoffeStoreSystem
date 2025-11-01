using Microsoft.EntityFrameworkCore;
using PRN232.Lab2.CoffeeStore.Repositories;
using PRN232.Lab2.CoffeeStore.Repositories.Entities;
using PRN232.Lab2.CoffeeStore.Services.Models;

namespace PRN232.Lab2.CoffeeStore.Services.AddonService
{
    public class AddonService : IAddonService
    {
        private readonly CoffeStoreDbContext _context;

        public AddonService(CoffeStoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<CoffeeAddonResponse>> GetAllAddonsAsync(bool? isActive = null)
        {
            var query = _context.CoffeeAddons.AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(a => a.IsActive == isActive.Value);
            }
            else
            {
                // Default: only return active addons
                query = query.Where(a => a.IsActive == true);
            }

            var addons = await query.ToListAsync();

            return addons.Select(a => new CoffeeAddonResponse
            {
                Id = a.Id,
                Name = a.Name,
                Price = a.Price,
                IsActive = a.IsActive
            }).ToList();
        }
    }
}


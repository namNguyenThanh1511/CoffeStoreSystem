using PRN232.Lab2.CoffeeStore.Services.Models;

namespace PRN232.Lab2.CoffeeStore.Services.AddonService
{
    public interface IAddonService
    {
        Task<List<CoffeeAddonResponse>> GetAllAddonsAsync(bool? isActive = null);
    }
}


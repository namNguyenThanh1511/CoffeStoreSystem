using PRN232.Lab2.CoffeeStore.Repositories.Entities;
using PRN232.Lab2.CoffeeStore.Repositories.GenericRepository;

namespace PRN232.Lab2.CoffeeStore.Repositories.CoffeeVariantRepository
{
    public class CoffeeVariantRepository : GenericRepository<CoffeeVariant>, ICoffeeVariantRepository
    {
        public CoffeeVariantRepository(CoffeStoreDbContext context) : base(context)
        {
        }
    }

}

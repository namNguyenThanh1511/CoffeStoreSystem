using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRN232.Lab2.CoffeeStore.Repositories.Entities;
using PRN232.Lab2.CoffeeStore.Repositories.GenericRepository;

namespace PRN232.Lab2.CoffeeStore.Repositories.CoffeeAddonRepository
{
    public class CoffeeAddonRepository : GenericRepository<CoffeeAddon>, ICoffeeAddonRepository
    {
        public CoffeeAddonRepository(CoffeStoreDbContext context) : base(context)
        {
        }
    }
}

using PRN232.Lab2.CoffeeStore.Repositories.GenericRepository;

namespace PRN232.Lab2.CoffeeStore.Repositories.ProductRepository
{
    public interface IProductRepository : IGenericRepository<Entities.Product>
    {
        Task<PagedList<Entities.Product>> GetAllProducts(IQueryable<Entities.Product> queryable, int pageNumber, int pageSize);
    }
}

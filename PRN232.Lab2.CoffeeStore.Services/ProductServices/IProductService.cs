using PRN232.Lab2.CoffeeStore.Repositories;
using PRN232.Lab2.CoffeeStore.Services.Models;

namespace PRN232.Lab2.CoffeeStore.Services.ProductServices
{
    public interface IProductService
    {
        Task<(List<ProductResponse>, MetaData metaData)> GetAllProductsAsync(ProductSearchParams searchParams);
        Task<ProductDetailsResponse> GetProductByIdAsync(Guid id);
        Task<ProductResponse> AddProductAsync(ProductCreationRequest request);
        Task UpdateProductAsync(Guid id, ProductUpdationRequest request);
        Task DeleteProductAsync(Guid id);
    }
}

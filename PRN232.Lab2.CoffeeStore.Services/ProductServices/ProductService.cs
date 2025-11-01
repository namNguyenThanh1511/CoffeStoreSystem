using Microsoft.EntityFrameworkCore;
using PRN232.Lab2.CoffeeStore.Repositories.Entities;
using PRN232.Lab2.CoffeeStore.Repositories.UnitOfWork;
using PRN232.Lab2.CoffeeStore.Services.Exceptions;
using PRN232.Lab2.CoffeeStore.Services.Models;

namespace PRN232.Lab2.CoffeeStore.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // get all products
        public async Task<List<ProductResponse>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync(
                q => q
                    .Where(p => p.IsActive)
                    .Include(p => p.Variants.Where(v => v.IsActive))
            );
            return MapToProductResponseList(products);
        }

        // get product by id
        public async Task<ProductDetailsResponse> GetProductByIdAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(
                id,
                q => q.Include(p => p.Variants)
            //.Include(p => p.ProductInMenus)
            //    .ThenInclude(pm => pm.Menu)
            );

            if (product == null)
                throw new NotFoundException("Product not found");

            return MapToProductDetailsResponse(product);
        }

        // add product
        public async Task<ProductResponse> AddProductAsync(ProductCreationRequest request)
        {
            Product product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Origin = request.Origin,
                RoastLevel = request.RoastLevel,
                BrewMethod = request.BrewMethod,
                ImageUrl = request.ImageUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Variants = request.Variants.Select(v => new CoffeeVariant
                {
                    Size = v.Size,
                    BasePrice = v.BasePrice,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }).ToList()
            };
            product = await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            var result = await _unitOfWork.Products.GetByIdAsync(product.Id, q => q.Include(p => p.Variants));
            if (result == null)
                throw new NotFoundException("Product not found after creation");
            return MapToProductResponse(result);
        }

        // update product
        public async Task UpdateProductAsync(Guid id, ProductUpdationRequest request)
        {
            var existingProduct = await _unitOfWork.Products.GetByIdAsync(id);
            if (existingProduct == null)
                throw new NotFoundException("Product not found");

            if (request.Name != null)
                existingProduct.Name = request.Name;
            if (request.Description != null)
                existingProduct.Description = request.Description;
            if (request.Origin != null)
                existingProduct.Origin = request.Origin;
            if (request.RoastLevel.HasValue)
                existingProduct.RoastLevel = request.RoastLevel.Value;
            if (request.BrewMethod.HasValue)
                existingProduct.BrewMethod = request.BrewMethod.Value;
            if (request.ImageUrl != null)
                existingProduct.ImageUrl = request.ImageUrl;
            if (request.IsActive.HasValue)
                existingProduct.IsActive = request.IsActive.Value;

            existingProduct.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Products.Update(existingProduct);
            await _unitOfWork.SaveChangesAsync();
        }

        // delete product
        public async Task DeleteProductAsync(Guid id)
        {
            var existingProduct = await _unitOfWork.Products.GetByIdAsync(id);
            if (existingProduct == null)
                throw new NotFoundException("Product not found");

            _unitOfWork.Products.Remove(existingProduct);
            await _unitOfWork.SaveChangesAsync();
        }

        // mapping helpers
        private ProductResponse MapToProductResponse(Product product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Variants = product.Variants
                    .Where(v => v.IsActive)
                    .Select(v => new CoffeeVariantResponse
                    {
                        Id = v.Id,
                        Size = v.Size.ToString(),
                        BasePrice = v.BasePrice
                    })
                    .ToList()
            };
        }

        private List<ProductResponse> MapToProductResponseList(IEnumerable<Product> products)
        {
            return products.Select(MapToProductResponse).ToList();
        }

        private ProductDetailsResponse MapToProductDetailsResponse(Product product)
        {
            return new ProductDetailsResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Origin = product.Origin,
                RoastLevel = product.RoastLevel.ToString(),
                BrewMethod = product.BrewMethod.ToString(),
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                Variants = product.Variants
                    .Where(v => v.IsActive)
                    .Select(v => new CoffeeVariantResponse
                    {
                        Id = v.Id,
                        Size = v.Size.ToString(),
                        BasePrice = v.BasePrice
                    })
                    .ToList()
            };
        }
    }
}

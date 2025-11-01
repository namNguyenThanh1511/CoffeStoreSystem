using Microsoft.AspNetCore.Mvc;
using PRN232.Lab2.CoffeeStore.API.Models;
using PRN232.Lab2.CoffeeStore.Services.Models;
using PRN232.Lab2.CoffeeStore.Services.ProductServices;

namespace PRN232.Lab2.CoffeeStore.API.Controllers
{
    [Route("api/products")]
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/product
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ProductResponse>>>> GetAllProducts([FromQuery] ProductSearchParams searchParams)
        {
            var (products, metaData) = await _productService.GetAllProductsAsync(searchParams);
            // Add pagination metadata to response header
            Response.Headers.Append("X-Pagination-CurrentPage", metaData.CurrentPage.ToString());
            Response.Headers.Append("X-Pagination-TotalPages", metaData.TotalPages.ToString());
            Response.Headers.Append("X-Pagination-PageSize", metaData.PageSize.ToString());
            Response.Headers.Append("X-Pagination-TotalCount", metaData.TotalCount.ToString());
            Response.Headers.Append("X-Pagination-HasPrevious", metaData.HasPrevious.ToString());
            Response.Headers.Append("X-Pagination-HasNext", metaData.HasNext.ToString());

            return Ok(products);
        }

        // GET: api/product/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductDetailsResponse>>> GetProductById(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return Ok(product);
        }

        // POST: api/product
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> AddProduct([FromBody] ProductCreationRequest request)
        {
            var createdProduct = await _productService.AddProductAsync(request);
            return Created(createdProduct, "Tạo product thành công");
        }
        // PUT: api/product/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse>> UpdateProduct(Guid id, [FromBody] ProductUpdationRequest request)
        {
            await _productService.UpdateProductAsync(id, request);
            return NoContent();
        }

        // DELETE: api/product/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteProduct(Guid id)
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }
    }
}

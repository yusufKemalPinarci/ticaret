using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Services;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync();
    Task<IEnumerable<ProductDto>> GetNewProductsAsync();
    Task<IEnumerable<ProductDto>> GetBackInStockProductsAsync();
    Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<ProductDto>> GetProductsByCollectionAsync(string collection);
    Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
    Task<ProductDto> CreateProductAsync(CreateProductDto createDto);
    Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateDto);
    Task<bool> DeleteProductAsync(int id);
}

using NotebookTherapy.Core.Entities;

namespace NotebookTherapy.Core.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetFeaturedProductsAsync();
    Task<IEnumerable<Product>> GetNewProductsAsync();
    Task<IEnumerable<Product>> GetBackInStockProductsAsync();
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<Product>> GetProductsByCollectionAsync(string collection);
    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
    Task<NotebookTherapy.Core.Models.PagedResult<Product>> GetFilteredProductsAsync(NotebookTherapy.Core.Models.ProductFilterOptions options);
}

using NotebookTherapy.Core.Entities;

namespace NotebookTherapy.Core.Interfaces;

public interface IProductVariantRepository : IRepository<ProductVariant>
{
    Task<IReadOnlyList<ProductVariant>> GetByProductIdAsync(int productId);
}

using NotebookTherapy.Core.Entities;

namespace NotebookTherapy.Core.Interfaces;

public interface ITaxRateRepository : IRepository<TaxRate>
{
    Task<TaxRate?> GetByRegionAsync(string region);
}

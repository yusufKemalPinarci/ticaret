using NotebookTherapy.Core.Entities;

namespace NotebookTherapy.Core.Interfaces;

public interface IShippingRateRepository : IRepository<ShippingRate>
{
    Task<ShippingRate?> GetRateAsync(string region, decimal weight);
}

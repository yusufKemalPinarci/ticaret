using Microsoft.EntityFrameworkCore;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;
using NotebookTherapy.Infrastructure.Data;

namespace NotebookTherapy.Infrastructure.Repositories;

public class ShippingRateRepository : Repository<ShippingRate>, IShippingRateRepository
{
    public ShippingRateRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ShippingRate?> GetRateAsync(string region, decimal weight)
    {
        return await _dbSet
            .Where(r => r.Region == region && weight >= r.WeightFrom && weight <= r.WeightTo && !r.IsDeleted)
            .OrderBy(r => r.WeightTo - r.WeightFrom)
            .FirstOrDefaultAsync();
    }
}

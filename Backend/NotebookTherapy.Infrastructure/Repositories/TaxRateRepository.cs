using Microsoft.EntityFrameworkCore;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;
using NotebookTherapy.Infrastructure.Data;

namespace NotebookTherapy.Infrastructure.Repositories;

public class TaxRateRepository : Repository<TaxRate>, ITaxRateRepository
{
    public TaxRateRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<TaxRate?> GetByRegionAsync(string region)
    {
        return await _dbSet.FirstOrDefaultAsync(r => r.Region == region && !r.IsDeleted);
    }
}

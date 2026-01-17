using NotebookTherapy.Core.Entities;

namespace NotebookTherapy.Core.Interfaces;

public interface IReviewRepository : IRepository<Review>
{
    Task<IEnumerable<Review>> GetByProductIdAsync(int productId);
    Task<Review> GetByIdWithUserAsync(int id);
    Task<double> GetAverageRatingAsync(int productId);
}

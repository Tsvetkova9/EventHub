using EventHub.Core.Entities;

namespace EventHub.Core.Interfaces
{
    public interface IReviewService
    {
        Task<Review?> GetByIdAsync(int id);

        Task<IEnumerable<Review>> GetReviewsByEventAsync(int eventId);

        Task<IEnumerable<Review>> GetReviewsByUserAsync(string userId);

        Task CreateAsync(Review review);

        Task DeleteAsync(int reviewId, string userId);

        Task<double> GetAverageRatingForEventAsync(int eventId);
    }
}

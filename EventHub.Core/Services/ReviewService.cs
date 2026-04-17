using EventHub.Core.Entities;
using EventHub.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Core.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IRepository<Review> _reviewRepository;

        public ReviewService(IRepository<Review> reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            return await _reviewRepository.GetQueryable()
                .Include(r => r.User)
                .Include(r => r.Event)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Review>> GetReviewsByEventAsync(int eventId)
        {
            return await _reviewRepository.GetQueryable()
                .Include(r => r.User)
                .Where(r => r.EventId == eventId)
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByUserAsync(string userId)
        {
            return await _reviewRepository.GetQueryable()
                .Include(r => r.Event)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();
        }

        public async Task CreateAsync(Review review)
        {
            review.CreatedOn = DateTime.UtcNow;
            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int reviewId, string userId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
                throw new InvalidOperationException("Review not found.");

            if (review.UserId != userId)
                throw new InvalidOperationException("You can only delete your own reviews.");

            _reviewRepository.Remove(review);
            await _reviewRepository.SaveChangesAsync();
        }

        public async Task<double> GetAverageRatingForEventAsync(int eventId)
        {
            var reviews = await _reviewRepository.GetQueryable()
                .Where(r => r.EventId == eventId)
                .ToListAsync();

            if (!reviews.Any())
                return 0;

            return reviews.Average(r => r.Rating);
        }
    }
}

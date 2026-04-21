using EventHub.Core.Entities;
using EventHub.Core.Interfaces;

namespace EventHub.Core.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IRepository<UserFavoriteEvent> _favoriteRepository;

        public FavoriteService(IRepository<UserFavoriteEvent> favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public async Task<bool> IsFavoriteAsync(int eventId, string userId)
        {
            var favorites = await _favoriteRepository.FindAsync(
                f => f.EventId == eventId && f.UserId == userId);
            return favorites.Any();
        }

        public async Task<IEnumerable<int>> GetFavoriteEventIdsAsync(string userId)
        {
            var favorites = await _favoriteRepository.FindAsync(f => f.UserId == userId);
            return favorites.Select(f => f.EventId);
        }

        public async Task ToggleFavoriteAsync(int eventId, string userId)
        {
            var existing = (await _favoriteRepository.FindAsync(
                f => f.EventId == eventId && f.UserId == userId)).FirstOrDefault();

            if (existing != null)
            {
                _favoriteRepository.Remove(existing);
            }
            else
            {
                await _favoriteRepository.AddAsync(new UserFavoriteEvent
                {
                    EventId = eventId,
                    UserId = userId
                });
            }

            await _favoriteRepository.SaveChangesAsync();
        }
    }
}

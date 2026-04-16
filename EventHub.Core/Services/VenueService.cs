using EventHub.Core.Entities;
using EventHub.Core.Interfaces;

namespace EventHub.Core.Services
{
    public class VenueService : IVenueService
    {
        private readonly IRepository<Venue> _venueRepository;

        public VenueService(IRepository<Venue> venueRepository)
        {
            _venueRepository = venueRepository;
        }

        public async Task<Venue?> GetByIdAsync(int id)
        {
            return await _venueRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Venue>> GetAllAsync()
        {
            return await _venueRepository.GetAllAsync();
        }

        public async Task CreateAsync(Venue venue)
        {
            await _venueRepository.AddAsync(venue);
            await _venueRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(Venue venue)
        {
            _venueRepository.Update(venue);
            await _venueRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var venue = await _venueRepository.GetByIdAsync(id);
            if (venue != null)
            {
                _venueRepository.Remove(venue);
                await _venueRepository.SaveChangesAsync();
            }
        }
    }
}

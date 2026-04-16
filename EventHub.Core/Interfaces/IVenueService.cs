using EventHub.Core.Entities;

namespace EventHub.Core.Interfaces
{
    public interface IVenueService
    {
        Task<Venue?> GetByIdAsync(int id);

        Task<IEnumerable<Venue>> GetAllAsync();

        Task CreateAsync(Venue venue);

        Task UpdateAsync(Venue venue);

        Task DeleteAsync(int id);
    }
}

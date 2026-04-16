using EventHub.Core.Entities;

namespace EventHub.Core.Interfaces
{
    public interface IEventService
    {
        Task<Event?> GetByIdAsync(int id);

        Task<Event?> GetByIdWithDetailsAsync(int id);

        Task<IEnumerable<Event>> GetAllAsync();

        Task<(IEnumerable<Event> Events, int TotalCount)> GetPagedAsync(
            string? searchTerm, int? categoryId, int page, int pageSize);

        Task<IEnumerable<Event>> GetUpcomingEventsAsync(int count);

        Task<IEnumerable<Event>> GetEventsByOrganizerAsync(string organizerId);

        Task CreateAsync(Event eventEntity);

        Task UpdateAsync(Event eventEntity);

        Task DeleteAsync(int id);

        Task<int> GetTotalCountAsync();
    }
}

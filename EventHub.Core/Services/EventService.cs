using EventHub.Core.Entities;
using EventHub.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Core.Services
{
    public class EventService : IEventService
    {
        private readonly IRepository<Event> _eventRepository;

        public EventService(IRepository<Event> eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<Event?> GetByIdAsync(int id)
        {
            return await _eventRepository.GetByIdAsync(id);
        }

        public async Task<Event?> GetByIdWithDetailsAsync(int id)
        {
            return await _eventRepository.GetQueryable()
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .Include(e => e.Organizer)
                .Include(e => e.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            return await _eventRepository.GetQueryable()
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Event> Events, int TotalCount)> GetPagedAsync(
            string? searchTerm, int? categoryId, int page, int pageSize)
        {
            var query = _eventRepository.GetQueryable()
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .Where(e => e.IsActive);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(e =>
                    e.Title.ToLower().Contains(term) ||
                    e.Description.ToLower().Contains(term) ||
                    e.Venue.City.ToLower().Contains(term));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(e => e.CategoryId == categoryId.Value);
            }

            var totalCount = await query.CountAsync();

            var events = await query
                .OrderByDescending(e => e.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (events, totalCount);
        }

        public async Task<IEnumerable<Event>> GetUpcomingEventsAsync(int count)
        {
            return await _eventRepository.GetQueryable()
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .Where(e => e.IsActive && e.StartDate > DateTime.UtcNow)
                .OrderBy(e => e.StartDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByOrganizerAsync(string organizerId)
        {
            return await _eventRepository.GetQueryable()
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .Where(e => e.OrganizerId == organizerId)
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();
        }

        public async Task CreateAsync(Event eventEntity)
        {
            eventEntity.CreatedOn = DateTime.UtcNow;
            await _eventRepository.AddAsync(eventEntity);
            await _eventRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(Event eventEntity)
        {
            _eventRepository.Update(eventEntity);
            await _eventRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);
            if (eventEntity != null)
            {
                eventEntity.IsActive = false;
                _eventRepository.Update(eventEntity);
                await _eventRepository.SaveChangesAsync();
            }
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _eventRepository.GetQueryable()
                .Where(e => e.IsActive)
                .CountAsync();
        }
    }
}

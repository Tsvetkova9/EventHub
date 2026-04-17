using EventHub.Core.Entities;
using EventHub.Core.Enums;
using EventHub.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Core.Services
{
    public class TicketService : ITicketService
    {
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IRepository<Event> _eventRepository;

        public TicketService(IRepository<Ticket> ticketRepository, IRepository<Event> eventRepository)
        {
            _ticketRepository = ticketRepository;
            _eventRepository = eventRepository;
        }

        public async Task<Ticket?> GetByIdAsync(int id)
        {
            return await _ticketRepository.GetQueryable()
                .Include(t => t.Event)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByUserAsync(string userId)
        {
            return await _ticketRepository.GetQueryable()
                .Include(t => t.Event)
                    .ThenInclude(e => e.Venue)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.PurchasedOn)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByEventAsync(int eventId)
        {
            return await _ticketRepository.GetQueryable()
                .Include(t => t.User)
                .Where(t => t.EventId == eventId)
                .OrderByDescending(t => t.PurchasedOn)
                .ToListAsync();
        }

        public async Task<Ticket> PurchaseTicketAsync(int eventId, string userId, int quantity)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
                throw new InvalidOperationException("Event not found.");

            if (!eventEntity.IsActive)
                throw new InvalidOperationException("This event is no longer active.");

            if (eventEntity.StartDate <= DateTime.UtcNow)
                throw new InvalidOperationException("This event has already started.");

            if (eventEntity.AvailableTickets < quantity)
                throw new InvalidOperationException("Not enough tickets available.");

            var ticket = new Ticket
            {
                EventId = eventId,
                UserId = userId,
                Quantity = quantity,
                TotalPrice = eventEntity.TicketPrice * quantity,
                PurchasedOn = DateTime.UtcNow,
                Status = TicketStatus.Active
            };

            eventEntity.AvailableTickets -= quantity;
            _eventRepository.Update(eventEntity);

            await _ticketRepository.AddAsync(ticket);
            await _ticketRepository.SaveChangesAsync();

            return ticket;
        }

        public async Task CancelTicketAsync(int ticketId, string userId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
                throw new InvalidOperationException("Ticket not found.");

            if (ticket.UserId != userId)
                throw new InvalidOperationException("You can only cancel your own tickets.");

            if (ticket.Status != TicketStatus.Active)
                throw new InvalidOperationException("Only active tickets can be cancelled.");

            ticket.Status = TicketStatus.Cancelled;
            _ticketRepository.Update(ticket);

            var eventEntity = await _eventRepository.GetByIdAsync(ticket.EventId);
            if (eventEntity != null)
            {
                eventEntity.AvailableTickets += ticket.Quantity;
                _eventRepository.Update(eventEntity);
            }

            await _ticketRepository.SaveChangesAsync();
        }

        public async Task<int> GetTotalTicketsSoldAsync()
        {
            return await _ticketRepository.GetQueryable()
                .Where(t => t.Status == TicketStatus.Active)
                .SumAsync(t => t.Quantity);
        }
    }
}

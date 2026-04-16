using EventHub.Core.Entities;

namespace EventHub.Core.Interfaces
{
    public interface ITicketService
    {
        Task<Ticket?> GetByIdAsync(int id);

        Task<IEnumerable<Ticket>> GetTicketsByUserAsync(string userId);

        Task<IEnumerable<Ticket>> GetTicketsByEventAsync(int eventId);

        Task<Ticket> PurchaseTicketAsync(int eventId, string userId, int quantity);

        Task CancelTicketAsync(int ticketId, string userId);

        Task<int> GetTotalTicketsSoldAsync();
    }
}

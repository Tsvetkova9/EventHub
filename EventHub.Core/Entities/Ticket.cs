using System.ComponentModel.DataAnnotations;
using EventHub.Core.Enums;

namespace EventHub.Core.Entities
{
    public class Ticket
    {
        public int Id { get; set; }

        public DateTime PurchasedOn { get; set; } = DateTime.UtcNow;

        [Range(1, 10)]
        public int Quantity { get; set; } = 1;

        public decimal TotalPrice { get; set; }

        public TicketStatus Status { get; set; } = TicketStatus.Active;

        // Foreign Keys
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
    }
}

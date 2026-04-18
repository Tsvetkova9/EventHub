using System.ComponentModel.DataAnnotations;

namespace EventHub.Web.Models.Ticket
{
    public class TicketListViewModel
    {
        public IEnumerable<TicketItemViewModel> Tickets { get; set; } = new List<TicketItemViewModel>();
    }

    public class TicketItemViewModel
    {
        public int Id { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime PurchasedOn { get; set; }
    }

    public class PurchaseTicketViewModel
    {
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public decimal TicketPrice { get; set; }
        public int AvailableTickets { get; set; }

        [Required]
        [Range(1, 10)]
        public int Quantity { get; set; } = 1;
    }
}

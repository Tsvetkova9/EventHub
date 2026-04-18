namespace EventHub.Web.Models.Home
{
    public class HomeViewModel
    {
        public IEnumerable<EventCardViewModel> UpcomingEvents { get; set; } = new List<EventCardViewModel>();
        public int TotalEvents { get; set; }
        public int TotalCategories { get; set; }
        public int TotalVenues { get; set; }
    }

    public class EventCardViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime StartDate { get; set; }
        public decimal TicketPrice { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }
}

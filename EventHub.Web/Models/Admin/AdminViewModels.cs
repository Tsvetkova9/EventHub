namespace EventHub.Web.Models.Admin
{
    public class DashboardViewModel
    {
        public int TotalEvents { get; set; }
        public int TotalUsers { get; set; }
        public int TotalTicketsSold { get; set; }
        public int TotalCategories { get; set; }
        public int TotalVenues { get; set; }
        public decimal TotalRevenue { get; set; }
        public IEnumerable<RecentEventViewModel> RecentEvents { get; set; } = new List<RecentEventViewModel>();
    }

    public class RecentEventViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int AvailableTickets { get; set; }
        public bool IsActive { get; set; }
    }
}

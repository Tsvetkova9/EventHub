using System.ComponentModel.DataAnnotations;

namespace EventHub.Web.Models.Event
{
    public class EventListViewModel
    {
        public IEnumerable<EventItemViewModel> Events { get; set; } = new List<EventItemViewModel>();
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<CategoryOptionViewModel> Categories { get; set; } = new List<CategoryOptionViewModel>();
    }

    public class EventItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TicketPrice { get; set; }
        public int AvailableTickets { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public bool IsFavorited { get; set; }
    }

    public class EventDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TicketPrice { get; set; }
        public int AvailableTickets { get; set; }
        public bool IsActive { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;
        public string VenueAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string OrganizerName { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public IEnumerable<ReviewItemViewModel> Reviews { get; set; } = new List<ReviewItemViewModel>();
    }

    public class ReviewItemViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }

    public class EventCreateViewModel
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(300)]
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Now.AddDays(7);

        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(8);

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Ticket Price")]
        public decimal TicketPrice { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Available Tickets")]
        public int AvailableTickets { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required]
        [Display(Name = "Venue")]
        public int VenueId { get; set; }

        public IEnumerable<CategoryOptionViewModel> Categories { get; set; } = new List<CategoryOptionViewModel>();
        public IEnumerable<VenueOptionViewModel> Venues { get; set; } = new List<VenueOptionViewModel>();
    }

    public class EventEditViewModel : EventCreateViewModel
    {
        public int Id { get; set; }
    }

    public class CategoryOptionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class VenueOptionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}

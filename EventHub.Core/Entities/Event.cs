using System.ComponentModel.DataAnnotations;

namespace EventHub.Core.Entities
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? ImageUrl { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TicketPrice { get; set; }

        public int AvailableTickets { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int VenueId { get; set; }
        public Venue Venue { get; set; } = null!;

        [Required]
        public string OrganizerId { get; set; } = string.Empty;
        public ApplicationUser Organizer { get; set; } = null!;

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public ICollection<UserFavoriteEvent> FavoritedBy { get; set; } = new List<UserFavoriteEvent>();
    }
}

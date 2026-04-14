using System.ComponentModel.DataAnnotations;

namespace EventHub.Core.Entities
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Rating { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
    }
}

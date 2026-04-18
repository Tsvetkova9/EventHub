using System.ComponentModel.DataAnnotations;

namespace EventHub.Web.Models.Review
{
    public class CreateReviewViewModel
    {
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
    }
}

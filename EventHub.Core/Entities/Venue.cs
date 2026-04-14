using System.ComponentModel.DataAnnotations;

namespace EventHub.Core.Entities
{
    public class Venue
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        [MaxLength(200)]
        public string? ImageUrl { get; set; }

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}

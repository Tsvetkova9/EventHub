using System.ComponentModel.DataAnnotations;

namespace EventHub.Core.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? ImageUrl { get; set; }

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}

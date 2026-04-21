namespace EventHub.Core.Entities
{
    public class UserFavoriteEvent
    {
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public int EventId { get; set; }
        public Event Event { get; set; } = null!;
    }
}

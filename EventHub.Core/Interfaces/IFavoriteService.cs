namespace EventHub.Core.Interfaces
{
    public interface IFavoriteService
    {
        /// <summary>Check if the given event is in the user's favorites.</summary>
        Task<bool> IsFavoriteAsync(int eventId, string userId);

        /// <summary>Return the set of event IDs the user has favorited.</summary>
        Task<IEnumerable<int>> GetFavoriteEventIdsAsync(string userId);

        /// <summary>Add to favorites if not present, remove if present.</summary>
        Task ToggleFavoriteAsync(int eventId, string userId);
    }
}

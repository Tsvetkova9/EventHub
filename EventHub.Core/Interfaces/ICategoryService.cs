using EventHub.Core.Entities;

namespace EventHub.Core.Interfaces
{
    public interface ICategoryService
    {
        Task<Category?> GetByIdAsync(int id);

        Task<IEnumerable<Category>> GetAllAsync();

        Task CreateAsync(Category category);

        Task UpdateAsync(Category category);

        Task DeleteAsync(int id);
    }
}

using EventHub.Core.Entities;
using EventHub.Core.Interfaces;

namespace EventHub.Core.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryService(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task CreateAsync(Category category)
        {
            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category != null)
            {
                _categoryRepository.Remove(category);
                await _categoryRepository.SaveChangesAsync();
            }
        }
    }
}

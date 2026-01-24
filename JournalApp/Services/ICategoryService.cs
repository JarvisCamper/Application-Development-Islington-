using JournalApp.Models.Category;

namespace JournalApp.Services
{
    /// <summary>
    /// Category service for retrieving categories
    /// </summary>
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
    }
}

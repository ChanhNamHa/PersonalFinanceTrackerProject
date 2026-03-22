using PersonalFinanceTracker.Application.DTOs;

namespace PersonalFinanceTracker.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync();

        Task<CategoryResponse?> GetCategoryByIdAsync(string id);

        Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request);

        Task<bool> DeleteCategoryAsync(string id);

        Task<bool> UpdateCategoryAsync(string id, CreateCategoryRequest request);
    }
}
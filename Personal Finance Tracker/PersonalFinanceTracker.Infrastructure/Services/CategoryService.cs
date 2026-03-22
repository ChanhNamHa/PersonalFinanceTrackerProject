using PersonalFinanceTracker.Application.DTOs;
using PersonalFinanceTracker.Application.Interfaces;
using PersonalFinanceTracker.Domain.Entities;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _uow;

    public CategoryService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync()
    {
        var categories = await _uow.Categories.GetAllAsync();
        return categories.Select(c => new CategoryResponse(c.Id.ToString(), c.Name, c.Type));
    }

    public async Task<CategoryResponse?> GetCategoryByIdAsync(string id)
    {
        var category = await _uow.Categories.GetByIdAsync(Guid.Parse(id)); // Sử dụng object id trong Generic Repo
        if (category == null) return null;

        return new CategoryResponse(category.Id.ToString(), category.Name, category.Type);
    }

    public async Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request)
    {
        // Logic kiểm tra trùng tên...
        var category = new Category
        {
            Name = request.Name,
            Type = request.Type
            // Id tự tăng nên không set ở đây
        };

        await _uow.Categories.AddAsync(category);
        await _uow.CompleteAsync();

        return new CategoryResponse(category.Id.ToString(), category.Name, category.Type);
    }

    public async Task<bool> DeleteCategoryAsync(string id)
    {
        var category = await _uow.Categories.GetByIdAsync(Guid.Parse(id));
        if (category == null) return false;

        _uow.Categories.Delete(category);
        return await _uow.CompleteAsync() > 0;
    }

    public async Task<bool> UpdateCategoryAsync(string id, CreateCategoryRequest request)
    {
        var category = await _uow.Categories.GetByIdAsync(Guid.Parse(id));
        if (category == null) return false;

        category.Name = request.Name;
        category.Type = request.Type;

        _uow.Categories.Update(category);
        return await _uow.CompleteAsync() > 0;
    }
}
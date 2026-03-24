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

    public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
    {
        // Lấy tất cả danh mục cho mọi người dùng
        var categories = await _uow.Categories.GetAllAsync();
        return categories.Select(c => new CategoryResponse(c.Id, c.Name, c.Type));
    }

    // Hàm tạo danh mục (Thường chỉ dành cho Admin hoặc dùng để Seed Data)
    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
    {
        var existing = await _uow.Categories.GetByNameAsync(request.Name);
        if (existing != null) throw new PersonalFinanceTracker.Application.Exceptions.ConflictException("Danh mục này đã tồn tại trong hệ thống.");

        var category = new Category
        {
            Name = request.Name,
            Type = request.Type
        };

        await _uow.Categories.AddAsync(category);
        await _uow.CompleteAsync();

        return new CategoryResponse(category.Id, category.Name, category.Type);
    }
}
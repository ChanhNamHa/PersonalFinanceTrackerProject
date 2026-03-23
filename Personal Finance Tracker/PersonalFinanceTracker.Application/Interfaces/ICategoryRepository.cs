using PersonalFinanceTracker.Application.Interfaces;
using PersonalFinanceTracker.Domain.Entities;

public interface ICategoryRepository : IGenericRepository<Category>
{
    // Lấy danh mục theo tên (không phân biệt User)
    Task<Category?> GetByNameAsync(string name);

    // Lấy danh sách danh mục theo Type (Expense/Income)
    Task<IEnumerable<Category>> GetByTypeAsync(string type);

}
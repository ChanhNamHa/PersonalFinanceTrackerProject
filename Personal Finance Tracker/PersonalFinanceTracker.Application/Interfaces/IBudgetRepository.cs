using PersonalFinanceTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceTracker.Application.Interfaces
{
    public interface IBudgetRepository : IGenericRepository<Budget>
    {
        // Tìm ngân sách đang hiệu lực cho 1 danh mục tại 1 thời điểm
        Task<Budget?> GetActiveBudgetAsync(Guid userId, Guid categoryId, DateTime date);

        // Lấy danh sách ngân sách kèm thông tin Category
        Task<IEnumerable<Budget>> GetUserBudgetsWithCategoryAsync(Guid userId);
        Task<bool> IsOverlappingAsync(Guid userId, Guid categoryId, DateTime startDate, DateTime endDate);
    }
}

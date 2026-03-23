using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Application.Interfaces;
using PersonalFinanceTracker.Domain.Entities;

namespace PersonalFinanceTracker.Infrastructure.Repositories
{
    public class BudgetRepository : GenericRepository<Budget>, IBudgetRepository
    {
        public BudgetRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Budget?> GetActiveBudgetAsync(Guid userId, Guid categoryId, DateTime date)
        {
            return await _context.Budgets
                .FirstOrDefaultAsync(b => b.UserId == userId &&
                                          b.CategoryId == categoryId &&
                                          date >= b.StartDate &&
                                          date <= b.EndDate);
        }

        public async Task<IEnumerable<Budget>> GetUserBudgetsWithCategoryAsync(Guid userId)
        {
            return await _context.Budgets
                .Include(b => b.Category)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.StartDate)
                .ToListAsync();
        }
        public async Task<bool> IsOverlappingAsync(Guid userId, Guid categoryId, DateTime startDate, DateTime endDate)
        {
            return await _context.Budgets.AnyAsync(b =>
                b.UserId == userId &&
                b.CategoryId == categoryId &&
                (
                    // Trường hợp 1: Ngày bắt đầu mới nằm trong khoảng của Budget cũ
                    (startDate >= b.StartDate && startDate <= b.EndDate) ||
                    // Trường hợp 2: Ngày kết thúc mới nằm trong khoảng của Budget cũ
                    (endDate >= b.StartDate && endDate <= b.EndDate) ||
                    // Trường hợp 3: Budget mới bao trùm hoàn toàn Budget cũ
                    (startDate <= b.StartDate && endDate >= b.EndDate)
                ));
        }
    }
}
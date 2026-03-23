using PersonalFinanceTracker.Application.Interfaces;
using PersonalFinanceTracker.Domain.Entities;

public interface ITransactionRepository : IGenericRepository<Transaction>
{
    // Lấy chi tiết 1 giao dịch kèm Category
    Task<Transaction?> GetTransactionByIdAsync(Guid id);

    // Lấy danh sách giao dịch của User kèm Category
    Task<IEnumerable<Transaction>> GetTransactionsWithCategoryAsync(Guid userId);

    // Tính tổng chi tiêu
    Task<decimal> GetTotalSpentAsync(Guid userId, Guid categoryId, DateTime startDate, DateTime endDate);
}
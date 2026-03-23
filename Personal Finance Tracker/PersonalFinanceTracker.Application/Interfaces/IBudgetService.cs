using PersonalFinanceTracker.Application.DTOs;

namespace PersonalFinanceTracker.Application.Interfaces
{
    public interface IBudgetService
    {
        // Tạo ngân sách mới
        Task<BudgetResponse> CreateBudgetAsync(Guid userId, CreateBudgetRequest request);

        // Lấy toàn bộ danh sách ngân sách của người dùng (kèm số tiền đã tiêu hiện tại)
        Task<IEnumerable<BudgetResponse>> GetUserBudgetsAsync(Guid userId);

        // Lấy chi tiết một ngân sách cụ thể
        Task<BudgetResponse?> GetBudgetByIdAsync(Guid userId, Guid budgetId);

        // Xóa ngân sách
        Task<bool> DeleteBudgetAsync(Guid userId, Guid budgetId);

        // Cập nhật hạn mức ngân sách
        Task<BudgetResponse> UpdateBudgetAsync(Guid userId, Guid budgetId, CreateBudgetRequest request);
    }
}
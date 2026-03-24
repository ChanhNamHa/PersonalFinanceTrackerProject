using PersonalFinanceTracker.Application.DTOs;
using PersonalFinanceTracker.Application.Interfaces;
using PersonalFinanceTracker.Domain.Common;
using PersonalFinanceTracker.Domain.Entities;

namespace PersonalFinanceTracker.Infrastructure.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IUnitOfWork _uow;

        public BudgetService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<BudgetResponse> CreateBudgetAsync(Guid userId, CreateBudgetRequest request)
        {
            //Validation logic ngày tháng
            if (request.EndDate <= request.StartDate)
                throw new ArgumentException("Ngày kết thúc phải lớn hơn ngày bắt đầu.");

            //Kiểm tra trùng lặp Budget (Mỗi User chỉ có 1 Budget cho 1 Category trong cùng 1 khoảng thời gian)
            var isOverlapping = await _uow.Budgets.IsOverlappingAsync(userId, request.CategoryId, request.StartDate, request.EndDate);
            if (isOverlapping)
                throw new InvalidOperationException("Đã tồn tại ngân sách cho danh mục này trong khoảng thời gian đã chọn.");
            var category = await _uow.Categories.GetByIdAsync(request.CategoryId);
            if (category.Type == CategoryTypes.Income) 
                throw new InvalidOperationException("Không thể tạo ngân sách cho danh mục thu nhập.");
            //Khởi tạo Entity
            var budget = new Budget
            {
                LimitAmount = request.LimitAmount,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                UserId = userId,
                CategoryId = request.CategoryId
            };

            await _uow.Budgets.AddAsync(budget);
            await _uow.CompleteAsync();

            return await MapToResponse(budget);
        }

        public async Task<IEnumerable<BudgetResponse>> GetUserBudgetsAsync(Guid userId)
        {
            var budgets = await _uow.Budgets.GetUserBudgetsWithCategoryAsync(userId);
            var results = new List<BudgetResponse>();

            foreach (var b in budgets)
            {
                results.Add(await MapToResponse(b));
            }
            return results;
        }

        public async Task<BudgetResponse?> GetBudgetByIdAsync(Guid userId, Guid budgetId)
        {
            var budget = await _uow.Budgets.GetByIdAsync(budgetId);

            // Security check: Đảm bảo budget thuộc về User
            if (budget == null || budget.UserId != userId) return null;

            return await MapToResponse(budget);
        }

        public async Task<bool> DeleteBudgetAsync(Guid userId, Guid budgetId)
        {
            var budget = await _uow.Budgets.GetByIdAsync(budgetId);
            if (budget == null || budget.UserId != userId) return false;

            _uow.Budgets.Delete(budget);
            return await _uow.CompleteAsync() > 0;
        }

        public async Task<BudgetResponse> UpdateBudgetAsync(Guid userId, Guid budgetId, CreateBudgetRequest request)
        {
            var budget = await _uow.Budgets.GetByIdAsync(budgetId);
            if (budget == null || budget.UserId != userId)
                throw new KeyNotFoundException("Không tìm thấy ngân sách.");

            // Cập nhật thông tin
            budget.LimitAmount = request.LimitAmount;
            budget.StartDate = request.StartDate;
            budget.EndDate = request.EndDate;
            budget.CategoryId = request.CategoryId;

            _uow.Budgets.Update(budget);
            await _uow.CompleteAsync();

            return await MapToResponse(budget);
        }

        // --- Helper Methods ---

        private async Task<BudgetResponse> MapToResponse(Budget b)
        {
            // Tính số tiền đã tiêu thực tế của User này trong Category chung này
            var spent = await _uow.Transactions.GetTotalSpentAsync(b.UserId, b.CategoryId, b.StartDate, b.EndDate);

            // Lấy tên Category (Vì DB không có UserId ở Category nên truy vấn rất thẳng hàng)
            var category = await _uow.Categories.GetByIdAsync(b.CategoryId);

            return new BudgetResponse(
                b.Id,
                b.LimitAmount,
                b.StartDate,
                b.EndDate,
                category?.Name ?? "Unknown",
                b.CategoryId,
                spent
            );
        }
    }
}
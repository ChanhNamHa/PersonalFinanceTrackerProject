using PersonalFinanceTracker.Application.DTOs;
using PersonalFinanceTracker.Application.Interfaces;
using PersonalFinanceTracker.Domain.Entities;
using PersonalFinanceTracker.Application.Exceptions;
public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _uow;

    public TransactionService(IUnitOfWork uow)
    {
        _uow = uow;
    }
    public async Task<TransactionDTO> UpdateTransactionAsync(Guid userId, Guid transactionId, UpdateTransactionRequest request)
    {
        var transaction = await _uow.Transactions.GetByIdAsync(transactionId);

        if (transaction == null || transaction.UserId != userId)
            throw new PersonalFinanceTracker.Application.Exceptions.NotFoundException("Giao dịch không tồn tại.");

        // Logic: Nếu đổi số tiền hoặc đổi Category, phải check lại Budget mới
        if (transaction.Amount != request.Amount || transaction.CategoryId != request.CategoryId)
        {
            await CheckBudgetAlert(userId, request.CategoryId, request.Amount);
        }

        transaction.Amount = request.Amount;
        transaction.Note = request.Note;
        transaction.TransactionDate = request.TransactionDate;
        transaction.CategoryId = request.CategoryId;

        _uow.Transactions.Update(transaction);
        await _uow.CompleteAsync();
        return new TransactionDTO(transaction.Id, transaction.Amount, transaction.Note, transaction.TransactionDate, "Updated", transaction.CategoryId);
    }
    public async Task<TransactionDTO> CreateTransactionAsync(Guid userId, CreateTransactionRequest request)
    {
        // 1. Kiểm tra Category có tồn tại không
        var category = await _uow.Categories.GetByIdAsync(request.CategoryId);
        if (category == null) throw new PersonalFinanceTracker.Application.Exceptions.NotFoundException("Danh mục không tồn tại.");

        // 2. Kiểm tra ngân sách (Budget Alert)
        await CheckBudgetAlert(userId, request.CategoryId, request.Amount);

        // 3. Map Request sang Entity
        var transaction = new Transaction
        {
            Amount = request.Amount,
            Note = request.Note,
            TransactionDate = request.TransactionDate,
            UserId = userId,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Transactions.AddAsync(transaction);
        await _uow.CompleteAsync();

        // 4. Trả về Record DTO (Sử dụng Target-typed new)
        return new TransactionDTO(
            transaction.Id,
            transaction.Amount,
            transaction.Note,
            transaction.TransactionDate,
            category.Name,
            category.Id);
    }

    public async Task<IEnumerable<TransactionDTO>> GetUserTransactionsAsync(Guid userId)
    {
        var transactions = await _uow.Transactions.GetTransactionsWithCategoryAsync(userId);

        return transactions.Select(t => new TransactionDTO(
            t.Id,
            t.Amount,
            t.Note,
            t.TransactionDate,
            t.Category.Name,
            t.CategoryId));
    }

    public async Task<TransactionDTO?> GetTransactionByIdAsync(Guid userId, Guid transactionId)
    {
        var transaction = await _uow.Transactions.GetTransactionByIdAsync(transactionId);

        if (transaction == null || transaction.UserId != userId) return null;

        return new TransactionDTO(
            transaction.Id,
            transaction.Amount,
            transaction.Note,
            transaction.TransactionDate,
            transaction.Category.Name,
            transaction.CategoryId);
    }

    public async Task<bool> DeleteTransactionAsync(Guid userId, Guid transactionId)
    {
        var transaction = await _uow.Transactions.GetByIdAsync(transactionId);

        // Security: Chỉ cho phép xóa nếu giao dịch thuộc về User đang đăng nhập
        if (transaction == null || transaction.UserId != userId) return false;

        _uow.Transactions.Delete(transaction);
        return await _uow.CompleteAsync() > 0;
    }

    private async Task CheckBudgetAlert(Guid userId, Guid categoryId, decimal newAmount)
    {
        var currentDate = DateTime.UtcNow;
        // Giả sử IBudgetRepository đã có hàm GetActiveBudgetAsync
        var budget = await _uow.Budgets.GetActiveBudgetAsync(userId, categoryId, currentDate);

        if (budget != null)
        {
            var totalSpent = await _uow.Transactions.GetTotalSpentAsync(userId, categoryId, budget.StartDate, budget.EndDate);

            if (totalSpent + newAmount > budget.LimitAmount)
            {
                //có thể tích hợp bắn Notification hoặc SignalR
                throw new BudgetExceededException(
                $"Giao dịch thất bại! Hạn mức còn lại: {budget.LimitAmount - totalSpent:N0}đ. " +
                $"Số tiền này vượt định mức {(totalSpent + newAmount) - budget.LimitAmount}đ. Vui lòng nâng hạn mức ngân sách trước khi tiếp tục.");
            }
            
        }
    }
}
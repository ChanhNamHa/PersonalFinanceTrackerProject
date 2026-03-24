using PersonalFinanceTracker.Domain.Entities;

namespace PersonalFinanceTracker.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        ITransactionRepository Transactions { get; }
        ICategoryRepository Categories { get; }
        IBudgetRepository Budgets { get; }

        Task<int> CompleteAsync(); // Thay thế cho SaveChangesAsync
    }
}
using PersonalFinanceTracker.Domain.Entities;

namespace PersonalFinanceTracker.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IGenericRepository<Transaction> Transactions { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Budget> Budgets { get; }

        Task<int> CompleteAsync(); // Thay thế cho SaveChangesAsync
    }
}
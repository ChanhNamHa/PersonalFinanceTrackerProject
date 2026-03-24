using PersonalFinanceTracker.Application.Interfaces;
using PersonalFinanceTracker.Domain.Entities;

namespace PersonalFinanceTracker.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IUserRepository Users { get; private set; }
        public ITransactionRepository Transactions { get; private set; }
        public ICategoryRepository Categories { get; private set; }
        public IBudgetRepository Budgets { get; private set; }
        public IRefreshTokenRepository RefreshTokens { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Transactions = new TransactionRepository(_context);
            Categories = new CategoryRepository(_context);
            Budgets = new BudgetRepository(_context);
            RefreshTokens = new RefreshTokenRepository(_context);
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
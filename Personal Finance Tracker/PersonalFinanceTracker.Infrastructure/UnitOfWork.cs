using PersonalFinanceTracker.Application.Interfaces;
using PersonalFinanceTracker.Domain.Entities;

namespace PersonalFinanceTracker.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IUserRepository Users { get; private set; }
        public IGenericRepository<Transaction> Transactions { get; private set; }
        public IGenericRepository<Category> Categories { get; private set; }
        public IGenericRepository<Budget> Budgets { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Transactions = new GenericRepository<Transaction>(_context);
            Categories = new GenericRepository<Category>(_context);
            Budgets = new GenericRepository<Budget>(_context);
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
using PersonalFinanceTracker.Application.Interfaces;
using PersonalFinanceTracker.Domain.Entities;

namespace PersonalFinanceTracker.Infrastructure.Repositories
{
    public class BudgetRepository : GenericRepository<User>, IBudgetRepository
    {
        public BudgetRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
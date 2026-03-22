using PersonalFinanceTracker.Domain.Entities;

namespace PersonalFinanceTracker.Application.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        // Phương thức đặc thù cho User
        Task<User?> GetByEmailAsync(string email);
    }
}
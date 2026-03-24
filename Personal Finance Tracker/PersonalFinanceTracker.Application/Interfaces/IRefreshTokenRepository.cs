using PersonalFinanceTracker.Domain.Entities;

namespace PersonalFinanceTracker.Application.Interfaces
{
    public interface IRefreshTokenRepository : IGenericRepository<UserRefreshToken>
    {
        Task<UserRefreshToken?> GetByTokenHashAsync(string tokenHash);
        Task<IEnumerable<UserRefreshToken>> GetByUserIdAsync(Guid userId);
    }
}

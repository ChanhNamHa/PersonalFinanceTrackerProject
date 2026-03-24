using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Application.Interfaces;
using PersonalFinanceTracker.Domain.Entities;

namespace PersonalFinanceTracker.Infrastructure.Repositories
{
    public class RefreshTokenRepository : GenericRepository<UserRefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<UserRefreshToken?> GetByTokenHashAsync(string tokenHash)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
        }

        public async Task<IEnumerable<UserRefreshToken>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet.Where(t => t.UserId == userId).ToListAsync();
        }
    }
}

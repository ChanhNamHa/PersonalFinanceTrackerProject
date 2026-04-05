using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Application.Interfaces;
using PersonalFinanceTracker.Domain.Common;

namespace PersonalFinanceTracker.Infrastructure.Repositories
{
    public class GenericRepository<Entity> : IGenericRepository<Entity> where Entity : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<Entity> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Entity>();
        }

        public async Task<Entity?> GetByIdAsync(Guid id, CancellationToken ct = default)
                => await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, ct);

        public async Task<IEnumerable<Entity>> GetAllAsync(CancellationToken ct = default)
                => await _dbSet.AsNoTracking().Where(e => !e.IsDeleted).ToListAsync(ct);
        public async Task AddAsync(Entity entity, CancellationToken ct = default)
                => await _dbSet.AddAsync(entity, ct);
        public void Update(Entity entity) => _dbSet.Update(entity);

        public void Delete(Entity entity)
        {
            entity.IsDeleted = true;
            _dbSet.Update(entity);
        }
    }
}
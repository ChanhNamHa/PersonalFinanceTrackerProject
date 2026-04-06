using PersonalFinanceTracker.Domain.Common;

namespace PersonalFinanceTracker.Application.Interfaces
{
    public interface IGenericRepository<Entity> where Entity : BaseEntity
    {
        Task<Entity?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Entity>> GetAllAsync(CancellationToken ct = default);
        Entity Add(Entity entity);
        void Update(Entity entity);
        void Delete(Entity entity);
    }
}
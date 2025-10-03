using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GestorStock.Services.Interfaces
{
    public interface ICrudService<TEntity, TKey> where TEntity : class
    {
        Task<List<TEntity>> GetAllAsync(CancellationToken ct = default);
        Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default);
        Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default);
        Task UpdateAsync(TEntity entity, CancellationToken ct = default);
        Task DeleteAsync(TKey id, CancellationToken ct = default);
    }
}

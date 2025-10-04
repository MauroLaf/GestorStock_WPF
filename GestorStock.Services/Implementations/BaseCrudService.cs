using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GestorStock.Data;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestorStock.Services.Implementations
{
    public abstract class BaseCrudService<TEntity, TKey> : ICrudService<TEntity, TKey> where TEntity : class
    {
        protected readonly StockDbContext _ctx;
        protected readonly DbSet<TEntity> _set;
        protected BaseCrudService(StockDbContext ctx) { _ctx = ctx; _set = _ctx.Set<TEntity>(); }

        public virtual Task<List<TEntity>> GetAllAsync(CancellationToken ct = default) =>
            _set.AsNoTracking().ToListAsync(ct);

        public virtual Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default) =>
            _set.FindAsync(new object?[] { id }, ct).AsTask();

        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default)
        { _set.Add(entity); await _ctx.SaveChangesAsync(ct); return entity; }

        public virtual async Task UpdateAsync(TEntity entity, CancellationToken ct = default)
        { _set.Update(entity); await _ctx.SaveChangesAsync(ct); }

        public virtual async Task DeleteAsync(TKey id, CancellationToken ct = default)
        { var e = await GetByIdAsync(id, ct); if (e != null) { _set.Remove(e); await _ctx.SaveChangesAsync(ct); } }
    }
}

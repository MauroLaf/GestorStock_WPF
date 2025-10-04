using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestorStock.Services.Implementations
{
    public class TipoSoporteService : ITipoSoporteService
    {
        private readonly StockDbContext _db;
        public TipoSoporteService(StockDbContext db) => _db = db;

        public async Task<List<TipoSoporte>> GetAllAsync()
            => await _db.TipoSoportes.AsNoTracking().OrderBy(t => t.Nombre).ToListAsync();

        public async Task<TipoSoporte?> GetByIdAsync(int id)
            => await _db.TipoSoportes.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

        public async Task<TipoSoporte> CreateAsync(TipoSoporte entity)
        {
            _db.TipoSoportes.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var current = await _db.TipoSoportes.FirstOrDefaultAsync(t => t.Id == id);
            if (current == null) return false;
            _db.TipoSoportes.Remove(current);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

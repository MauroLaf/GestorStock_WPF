using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestorStock.Services.Implementations
{
    public class FamiliaService : IFamiliaService
    {
        private readonly StockDbContext _db;
        public FamiliaService(StockDbContext db) => _db = db;

        public async Task<List<Familia>> GetAllAsync()
            => await _db.Familias.AsNoTracking().OrderBy(f => f.Nombre).ToListAsync();

        public async Task<Familia?> GetByIdAsync(int id)
            => await _db.Familias.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);

        public async Task<Familia> CreateAsync(Familia entity)
        {
            _db.Familias.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var current = await _db.Familias.FirstOrDefaultAsync(f => f.Id == id);
            if (current == null) return false;
            _db.Familias.Remove(current);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

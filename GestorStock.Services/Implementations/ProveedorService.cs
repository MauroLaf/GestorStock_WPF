using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestorStock.Services.Implementations
{
    public class ProveedorService : IProveedorService
    {
        private readonly StockDbContext _db;
        public ProveedorService(StockDbContext db) => _db = db;

        public async Task<List<Proveedor>> GetAllAsync()
            => await _db.Proveedores.AsNoTracking().OrderBy(p => p.Nombre).ToListAsync();

        public async Task<Proveedor?> GetByIdAsync(int id)
            => await _db.Proveedores.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Proveedor> CreateAsync(Proveedor entity)
        {
            _db.Proveedores.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var current = await _db.Proveedores.FirstOrDefaultAsync(p => p.Id == id);
            if (current == null) return false;
            _db.Proveedores.Remove(current);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

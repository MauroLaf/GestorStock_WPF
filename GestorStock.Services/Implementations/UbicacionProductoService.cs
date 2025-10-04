using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestorStock.Services.Implementations
{
    public class UbicacionProductoService : IUbicacionProductoService
    {
        private readonly StockDbContext _db;
        public UbicacionProductoService(StockDbContext db) => _db = db;

        public async Task<List<UbicacionProducto>> GetAllAsync()
            => await _db.UbicacionProductos.AsNoTracking().OrderBy(u => u.Nombre).ToListAsync();

        public async Task<List<UbicacionProducto>> GetByFamiliaAsync(int familiaId)
            => await _db.UbicacionProductos.AsNoTracking()
                .Where(u => u.FamiliaId == familiaId)
                .OrderBy(u => u.Nombre)
                .ToListAsync();

        public async Task<UbicacionProducto?> GetByIdAsync(int id)
            => await _db.UbicacionProductos.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

        public async Task<UbicacionProducto> CreateAsync(UbicacionProducto entity)
        {
            _db.UbicacionProductos.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var current = await _db.UbicacionProductos.FirstOrDefaultAsync(u => u.Id == id);
            if (current == null) return false;
            _db.UbicacionProductos.Remove(current);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

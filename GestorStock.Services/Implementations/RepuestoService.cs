using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestorStock.Services.Implementations
{
    public class RepuestoService : IRepuestoService
    {
        private readonly StockDbContext _db;

        public RepuestoService(StockDbContext db)
        {
            _db = db;
        }

        public async Task<List<Repuesto>> GetAllAsync()
        {
            return await _db.Repuestos
                .AsNoTracking()
                .Include(r => r.Familia)
                .Include(r => r.UbicacionProducto)
                .Include(r => r.Proveedor)
                .Include(r => r.TipoSoporte)
                .ToListAsync();
        }

        public async Task<Repuesto?> GetByIdAsync(int id)
        {
            return await _db.Repuestos
                .AsNoTracking()
                .Include(r => r.Familia)
                .Include(r => r.UbicacionProducto)
                .Include(r => r.Proveedor)
                .Include(r => r.TipoSoporte)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Repuesto> AddAsync(Repuesto entity)
        {
            _db.Repuestos.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateAsync(Repuesto entity)
        {
            var exists = await _db.Repuestos.AnyAsync(r => r.Id == entity.Id);
            if (!exists) return false;

            _db.Repuestos.Update(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.Repuestos.FirstOrDefaultAsync(r => r.Id == id);
            if (entity == null) return false;

            _db.Repuestos.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

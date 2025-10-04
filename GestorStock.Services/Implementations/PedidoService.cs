using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestorStock.Services.Implementations
{
    public class PedidoService : IPedidoService
    {
        private readonly StockDbContext _db;

        public PedidoService(StockDbContext db)
        {
            _db = db;
        }

        public async Task<List<Pedido>> GetAllAsync()
        {
            // Ya incluye detalle básico
            return await _db.Pedidos
                .AsNoTracking()
                .Include(p => p.Familia)
                .Include(p => p.Repuestos).ThenInclude(r => r.UbicacionProducto)
                .Include(p => p.Repuestos).ThenInclude(r => r.TipoSoporte)
                .ToListAsync();
        }

        public async Task<List<Pedido>> GetAllWithDetalleAsync()
        {
            // “Con todo”: Familia + Repuestos + todas las FKs de repuesto
            return await _db.Pedidos
                .AsNoTracking()
                .Include(p => p.Familia)
                .Include(p => p.Repuestos).ThenInclude(r => r.Familia)
                .Include(p => p.Repuestos).ThenInclude(r => r.UbicacionProducto)
                .Include(p => p.Repuestos).ThenInclude(r => r.Proveedor)
                .Include(p => p.Repuestos).ThenInclude(r => r.TipoSoporte)
                .ToListAsync();
        }

        public async Task<Pedido?> GetByIdAsync(int id)
        {
            return await _db.Pedidos
                .AsNoTracking()
                .Include(p => p.Familia)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Pedido?> GetWithDetalleAsync(int id)
        {
            return await _db.Pedidos
                .AsNoTracking()
                .Include(p => p.Familia)
                .Include(p => p.Repuestos).ThenInclude(r => r.Familia)
                .Include(p => p.Repuestos).ThenInclude(r => r.UbicacionProducto)
                .Include(p => p.Repuestos).ThenInclude(r => r.Proveedor)
                .Include(p => p.Repuestos).ThenInclude(r => r.TipoSoporte)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Pedido> CreateAsync(Pedido pedido)
        {
            _db.Pedidos.Add(pedido);
            await _db.SaveChangesAsync();
            return pedido;
        }

        // Alias para compatibilidad con código existente
        public Task<Pedido> AddAsync(Pedido pedido) => CreateAsync(pedido);

        public async Task<bool> UpdateAsync(Pedido pedido)
        {
            var exists = await _db.Pedidos.AnyAsync(p => p.Id == pedido.Id);
            if (!exists) return false;

            _db.Pedidos.Update(pedido);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.Pedidos.FirstOrDefaultAsync(p => p.Id == id);
            if (entity == null) return false;

            _db.Pedidos.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

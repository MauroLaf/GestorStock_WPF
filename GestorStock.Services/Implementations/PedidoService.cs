using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorStock.Services.Implementations
{
    public class PedidoService : IPedidoService
    {
        private readonly StockDbContext _context;

        public PedidoService(StockDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Pedido>> GetAllPedidosAsync()
        {
            return await _context.Pedidos
                                 .Include(p => p.Items)
                                 .ThenInclude(i => i.TipoExplotacion)
                                 .Include(p => p.Items)
                                 .ThenInclude(i => i.Repuestos)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Pedido>> GetAllPedidosWithDetailsAsync()
        {
            return await _context.Pedidos
                                 .Include(p => p.Items)
                                 .ThenInclude(i => i.TipoExplotacion)
                                 .Include(p => p.Items)
                                 .ThenInclude(i => i.TipoSoporte) 
                                 .Include(p => p.Items)
                                 .ThenInclude(i => i.Repuestos)
                                 .ThenInclude(r => r.TipoRepuesto)
                                 .ToListAsync();
        }

        public async Task<Pedido?> GetPedidoByIdAsync(int id)
        {
            // Añade la línea para cargar la relación TipoRepuesto en la búsqueda por ID.
            return await _context.Pedidos
                                 .Include(p => p.Items)
                                 .ThenInclude(i => i.TipoExplotacion)
                                 .Include(p => p.Items)
                                 .ThenInclude(i => i.TipoSoporte) 
                                 .Include(p => p.Items)
                                 .ThenInclude(i => i.Repuestos)
                                 .ThenInclude(r => r.TipoRepuesto) 
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task CreatePedidoAsync(Pedido pedido)
        {
            foreach (var item in pedido.Items)
            {
                if (item.TipoSoporte != null && item.TipoSoporte.Id > 0)
                {
                    _context.Entry(item.TipoSoporte).State = EntityState.Unchanged;
                }

                if (item.TipoExplotacion != null && item.TipoExplotacion.Id > 0)
                {
                    _context.Entry(item.TipoExplotacion).State = EntityState.Unchanged;
                }

                foreach (var repuesto in item.Repuestos)
                {
                    if (repuesto.TipoRepuesto != null && repuesto.TipoRepuesto.Id > 0)
                    {
                        _context.Entry(repuesto.TipoRepuesto).State = EntityState.Unchanged;
                    }
                }
            }

            await _context.Pedidos.AddAsync(pedido);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePedidoAsync(Pedido pedido)
        {
            _context.Entry(pedido).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeletePedidoAsync(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido != null)
            {
                _context.Pedidos.Remove(pedido);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteRepuestoAsync(int repuestoId)
        {
            var repuesto = await _context.Repuestos.FindAsync(repuestoId);
            if (repuesto != null)
            {
                _context.Repuestos.Remove(repuesto);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
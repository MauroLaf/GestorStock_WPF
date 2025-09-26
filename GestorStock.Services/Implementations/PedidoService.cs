using GestorStock.Data.Repositories;
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
                                 .ThenInclude(i => i.Repuestos)
                                 .ToListAsync();
        }
        public async Task<Pedido?> GetPedidoByIdAsync(int id)
        {
            return await _context.Pedidos
                                 .Include(p => p.Items)
                                 .ThenInclude(i => i.TipoExplotacion)
                                 .Include(p => p.Items)
                                 .ThenInclude(i => i.Repuestos)
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task CreatePedidoAsync(Pedido pedido)
        {
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
    }
}
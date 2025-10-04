using System.Threading;
using System.Threading.Tasks;
using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestorStock.Services.Implementations
{
    public class PedidoService : BaseCrudService<Pedido, int>, IPedidoService
    {
        public PedidoService(StockDbContext ctx) : base(ctx) { }

        public async Task<Pedido?> GetWithDetalleAsync(int id, CancellationToken ct = default)
            => await _ctx.Pedidos
                         .Include(p => p.Repuestos)
                         .FirstOrDefaultAsync(p => p.Id == id, ct);
        public Task<List<Pedido>> GetAllWithDetalleAsync() =>
        _ctx.Pedidos
            .Include(p => p.Repuestos)
                .ThenInclude(r => r.UbicacionProducto)
                    .ThenInclude(u => u.Familia)
            .Include(p => p.Repuestos)
                .ThenInclude(r => r.TipoSoporte)
            .AsNoTracking()
            .ToListAsync();

        public Task<Pedido?> GetWithDetalleAsync(int id) =>
            _ctx.Pedidos
                .Include(p => p.Repuestos)
                    .ThenInclude(r => r.UbicacionProducto)
                        .ThenInclude(u => u.Familia)
                .Include(p => p.Repuestos)
                    .ThenInclude(r => r.TipoSoporte)
                .FirstOrDefaultAsync(p => p.Id == id);
    }
}

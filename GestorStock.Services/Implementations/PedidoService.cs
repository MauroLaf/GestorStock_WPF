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
    }
}

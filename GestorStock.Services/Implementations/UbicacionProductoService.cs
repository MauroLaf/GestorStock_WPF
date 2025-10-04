using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestorStock.Services.Implementations
{
    public class UbicacionProductoService : BaseCrudService<UbicacionProducto, int>, IUbicacionProductoService
    {
        public UbicacionProductoService(StockDbContext ctx) : base(ctx) { }

        public Task<List<UbicacionProducto>> GetByFamiliaAsync(int familiaId, CancellationToken ct = default) =>
            _ctx.UbicacionProductos.Where(u => u.FamiliaId == familiaId)
                .OrderBy(u => u.Nombre).AsNoTracking().ToListAsync(ct);
    }
}

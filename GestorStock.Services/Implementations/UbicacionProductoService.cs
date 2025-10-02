using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorStock.Services.Implementations
{
    public class UbicacionProductoService : IUbicacionProductoService
    {
        private readonly IDbContextFactory<StockDbContext> _contextFactory;

        public UbicacionProductoService(IDbContextFactory<StockDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<UbicacionProducto>> GetAllUbicacionProductosAsync()
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.UbicacionProductos.ToListAsync();
            }
        }

        public async Task<IEnumerable<UbicacionProducto>> GetUbicacionProductosByFamiliaIdAsync(int familiaId)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.UbicacionProductos
                                     .Where(up => up.FamiliaId == familiaId)
                                     .ToListAsync();
            }
        }

        public async Task<UbicacionProducto> CreateUbicacionProductoAsync(UbicacionProducto ubicacionProducto)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                context.UbicacionProductos.Add(ubicacionProducto);
                await context.SaveChangesAsync();
                return ubicacionProducto;
            }
        }
    }
}
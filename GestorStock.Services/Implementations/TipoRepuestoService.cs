using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorStock.Services.Implementations
{
    public class TipoRepuestoService : ITipoRepuestoService
    {
        private readonly IDbContextFactory<StockDbContext> _contextFactory;

        public TipoRepuestoService(IDbContextFactory<StockDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<TipoRepuesto>> GetAllTipoRepuestoAsync()
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.TipoRepuestos.ToListAsync();
            }
        }
    }
}
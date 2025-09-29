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
        private readonly StockDbContext _context;

        public TipoRepuestoService(StockDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TipoRepuesto>> GetAllTipoRepuestoAsync()
        {
            return await _context.TipoRepuestos.ToListAsync();
        }
    }
}
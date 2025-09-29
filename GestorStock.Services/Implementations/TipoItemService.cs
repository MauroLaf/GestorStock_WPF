using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorStock.Services.Implementations
{
    public class TipoItemService : ITipoItemService
    {
        private readonly StockDbContext _context;

        public TipoItemService(StockDbContext context)
        {
            _context = context;
        }

        public async Task<List<TipoSoporte>> GetAllTipoItemAsync()
        {
            return await _context.TiposItem.ToListAsync();
        }
    }
}
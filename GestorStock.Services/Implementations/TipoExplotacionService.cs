using GestorStock.Data.Repositories;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorStock.Services.Implementations
{
    public class TipoExplotacionService : ITipoExplotacionService
    {
        private readonly StockDbContext _context;

        public TipoExplotacionService(StockDbContext context)
        {
            _context = context;
        }

        public async Task<List<TipoExplotacion>> GetAllTipoExplotacionAsync()
        {
            return await _context.TipoExplotaciones.ToListAsync();
        }

        public async Task<TipoExplotacion?> GetTipoExplotacionByIdAsync(int id)
        {
            return await _context.TipoExplotaciones.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task CreateTipoExplotacionAsync(TipoExplotacion tipo)
        {
            await _context.TipoExplotaciones.AddAsync(tipo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTipoExplotacionAsync(TipoExplotacion tipo)
        {
            _context.Entry(tipo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTipoExplotacionAsync(int id)
        {
            var tipo = await _context.TipoExplotaciones.FindAsync(id);
            if (tipo != null)
            {
                _context.TipoExplotaciones.Remove(tipo);
                await _context.SaveChangesAsync();
            }
        }
    }
}
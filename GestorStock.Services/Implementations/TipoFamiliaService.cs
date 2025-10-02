using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorStock.Services.Implementations
{
    public class TipoFamiliaService : ITipoFamiliaService
    {
        private readonly IDbContextFactory<StockDbContext> _contextFactory;

        public TipoFamiliaService(IDbContextFactory<StockDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Familia>> GetAllTipoFamiliaAsync()
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.Familias.ToListAsync();
            }
        }

        public async Task<Familia?> GetTipoFamiliaByIdAsync(int id)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.Familias.FirstOrDefaultAsync(t => t.Id == id);
            }
        }

        public async Task CreateTipoFamiliaAsync(Familia tipo)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                await context.Familias.AddAsync(tipo);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateTipoFamiliaAsync(Familia tipo)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                context.Entry(tipo).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteTipoFamiliaAsync(int id)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var tipo = await context.Familias.FindAsync(id);
                if (tipo != null)
                {
                    context.Familias.Remove(tipo);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
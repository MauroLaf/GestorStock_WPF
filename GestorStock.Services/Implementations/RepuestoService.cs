using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorStock.Services.Implementations
{
    public class RepuestoService : IRepuestoService
    {
        private readonly IDbContextFactory<StockDbContext> _contextFactory;

        public RepuestoService(IDbContextFactory<StockDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<Repuesto>> GetAllRepuestosAsync()
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.Repuestos.ToListAsync();
            }
        }

        public async Task<Repuesto?> GetRepuestoByIdAsync(int id)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.Repuestos.FindAsync(id);
            }
        }

        public async Task CreateRepuestoAsync(Repuesto repuesto)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                await context.Repuestos.AddAsync(repuesto);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateRepuestoAsync(Repuesto repuesto)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var existingRepuesto = await context.Repuestos
                    .Include(r => r.TipoRepuesto)
                    .FirstOrDefaultAsync(r => r.Id == repuesto.Id);

                if (existingRepuesto != null)
                {
                    existingRepuesto.Nombre = repuesto.Nombre;
                    existingRepuesto.Cantidad = repuesto.Cantidad;
                    existingRepuesto.Precio = repuesto.Precio;

                    if (repuesto.TipoRepuesto != null)
                    {
                        var tipoRepuestoFromDb = await context.TipoRepuestos.FindAsync(repuesto.TipoRepuesto.Id);
                        if (tipoRepuestoFromDb != null)
                        {
                            existingRepuesto.TipoRepuesto = tipoRepuestoFromDb;
                        }
                    }

                    context.Repuestos.Update(existingRepuesto);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteRepuestoAsync(int id)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var repuesto = await context.Repuestos.FindAsync(id);
                if (repuesto != null)
                {
                    context.Repuestos.Remove(repuesto);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task DescontarUnidadAsync(int id, int cantidad)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var repuesto = await context.Repuestos.FindAsync(id);
                if (repuesto != null)
                {
                    repuesto.Cantidad -= cantidad;
                    context.Repuestos.Update(repuesto);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
using GestorStock.Data.Repositories;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorStock.Services.Implementations
{
    public class RepuestoService : IRepuestoService
    {
        private readonly StockDbContext _context;

        public RepuestoService(StockDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Repuesto>> GetAllRepuestosAsync()
        {
            return await _context.Repuestos.ToListAsync();
        }

        public async Task<Repuesto?> GetRepuestoByIdAsync(int id)
        {
            return await _context.Repuestos.FindAsync(id);
        }

        public async Task CreateRepuestoAsync(Repuesto repuesto)
        {
            await _context.Repuestos.AddAsync(repuesto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRepuestoAsync(Repuesto repuesto)
        {
            // Carga el repuesto existente de la base de datos, incluyendo su TipoRepuesto.
            var existingRepuesto = await _context.Repuestos
                .Include(r => r.TipoRepuesto) // Asegura que el TipoRepuesto se cargue
                .FirstOrDefaultAsync(r => r.Id == repuesto.Id);

            if (existingRepuesto != null)
            {
                // Actualiza las propiedades escalares (Nombre, Cantidad).
                existingRepuesto.Nombre = repuesto.Nombre;
                existingRepuesto.Cantidad = repuesto.Cantidad;

                // Carga el TipoRepuesto de la base de datos por el ID del objeto que viene de la UI.
                // Esto evita errores de "tracking" de Entity Framework.
                if (repuesto.TipoRepuesto != null)
                {
                    var tipoRepuestoFromDb = await _context.TipoRepuestos
                        .FindAsync(repuesto.TipoRepuesto.Id);

                    if (tipoRepuestoFromDb != null)
                    {
                        // Asigna el objeto cargado al repuesto existente.
                        existingRepuesto.TipoRepuesto = tipoRepuestoFromDb;
                    }
                }

                // Le indicamos al contexto que el objeto ha sido modificado.
                _context.Repuestos.Update(existingRepuesto);

                // Guardamos los cambios en la base de datos.
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteRepuestoAsync(int id)
        {
            var repuesto = await _context.Repuestos.FindAsync(id);
            if (repuesto != null)
            {
                _context.Repuestos.Remove(repuesto);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DescontarUnidadAsync(int id, int cantidad)
        {
            var repuesto = await _context.Repuestos.FindAsync(id);
            if (repuesto != null)
            {
                repuesto.Cantidad -= cantidad;
                _context.Repuestos.Update(repuesto);
                await _context.SaveChangesAsync();
            }
        }
    }
}
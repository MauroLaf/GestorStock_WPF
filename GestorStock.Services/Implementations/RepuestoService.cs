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
            _context.Entry(repuesto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
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
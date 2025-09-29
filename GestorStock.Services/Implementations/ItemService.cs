using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorStock.Services.Implementations
{
    public class ItemService : IItemService
    {
        private readonly StockDbContext _context;

        public ItemService(StockDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            return await _context.Items
                                   .Include(i => i.TipoExplotacion)
                                   .Include(i => i.TipoSoporte)
                                   .Include(i => i.Repuestos)
                                       .ThenInclude(r => r.TipoRepuesto)
                                   .ToListAsync();
        }

        public async Task<Item?> GetItemByIdAsync(int id)
        {
            return await _context.Items
                                   .Include(i => i.TipoExplotacion)
                                   .Include(i => i.TipoSoporte)
                                   .Include(i => i.Repuestos)
                                       .ThenInclude(r => r.TipoRepuesto)
                                   .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task CreateItemAsync(Item item)
        {
            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateItemAsync(Item item)
        {
            var existingItem = await _context.Items
                                   .Include(i => i.Repuestos)
                                   .FirstOrDefaultAsync(i => i.Id == item.Id);

            if (existingItem != null)
            {
                // 1. Actualiza las propiedades básicas del ítem.
                existingItem.NombreUbicacion = item.NombreUbicacion;
                existingItem.TipoExplotacion = item.TipoExplotacion;
                existingItem.TipoSoporte = item.TipoSoporte;

                // 2. Sincroniza la colección de repuestos.
                var incomingRepuestos = item.Repuestos.ToList();

                // Identifica los repuestos a eliminar.
                var repuestosToDelete = existingItem.Repuestos
                    .Where(r => !incomingRepuestos.Any(inc => inc.Id == r.Id))
                    .ToList();

                // Elimina los repuestos que ya no existen.
                _context.Repuestos.RemoveRange(repuestosToDelete);

                // Recorre la lista de repuestos que vienen de la UI.
                foreach (var incomingRepuesto in incomingRepuestos)
                {
                    var existingRepuesto = existingItem.Repuestos
                        .FirstOrDefault(r => r.Id == incomingRepuesto.Id);

                    if (existingRepuesto != null)
                    {
                        // Repuesto existente: actualiza sus propiedades.
                        existingRepuesto.Nombre = incomingRepuesto.Nombre;
                        existingRepuesto.Cantidad = incomingRepuesto.Cantidad;
                        existingRepuesto.TipoRepuesto = incomingRepuesto.TipoRepuesto;
                    }
                    else
                    {
                        // Nuevo repuesto: agrégalo a la colección.
                        existingItem.Repuestos.Add(incomingRepuesto);
                    }
                }

                // 3. Guarda todos los cambios en la base de datos.
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteItemAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
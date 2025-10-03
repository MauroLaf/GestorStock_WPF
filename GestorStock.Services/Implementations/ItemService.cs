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
        private readonly IDbContextFactory<StockDbContext> _contextFactory;

        public ItemService(IDbContextFactory<StockDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.Items
                    .Include(i => i.UbicacionProducto!)
                        .ThenInclude(up => up.Familia)
                    .Include(i => i.TipoSoporte!)
                    .Include(i => i.Repuestos!)
                    .ToListAsync();
            }
        }

        public async Task<Item?> GetItemByIdAsync(int id)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.Items
                    .Include(i => i.UbicacionProducto!)
                        .ThenInclude(up => up.Familia)
                    .Include(i => i.TipoSoporte!)
                    .Include(i => i.Repuestos!)
                    .FirstOrDefaultAsync(i => i.Id == id);
            }
        }

        public async Task<Item?> GetItemWithAllRelationsAsync(int id)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.Items
                    .Include(i => i.UbicacionProducto!)
                        .ThenInclude(up => up.Familia)
                    .Include(i => i.TipoSoporte!) 
                    .Include(i => i.Repuestos!)
                        .ThenInclude(r => r.TipoRepuesto!)
                    .FirstOrDefaultAsync(i => i.Id == id);
            }
        }

        public async Task CreateItemAsync(Item item)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                context.Items.Add(item);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateItemAsync(Item item)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var existingItem = await context.Items
                    .Include(i => i.Repuestos!)
                    .FirstOrDefaultAsync(i => i.Id == item.Id);

                if (existingItem != null)
                {
                    context.Entry(existingItem).CurrentValues.SetValues(item);

                    var repuestosToRemove = existingItem.Repuestos!.Where(r => !item.Repuestos!.Any(nr => nr.Id == r.Id)).ToList();
                    foreach (var repuesto in repuestosToRemove)
                    {
                        context.Repuestos.Remove(repuesto);
                    }

                    foreach (var repuesto in item.Repuestos!)
                    {
                        var existingRepuesto = existingItem.Repuestos.FirstOrDefault(r => r.Id == repuesto.Id);
                        if (existingRepuesto != null)
                        {
                            context.Entry(existingRepuesto).CurrentValues.SetValues(repuesto);
                        }
                        else
                        {
                            existingItem.Repuestos.Add(repuesto);
                        }
                    }
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteItemAsync(int id)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var itemToDelete = await context.Items.FindAsync(id);
                if (itemToDelete != null)
                {
                    context.Items.Remove(itemToDelete);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorStock.Services.Implementations
{
    public class PedidoService : IPedidoService
    {
        private readonly IDbContextFactory<StockDbContext> _contextFactory;

        // ¡CORRECCIÓN CLAVE! El nombre del constructor debe coincidir con el de la clase
        public PedidoService(IDbContextFactory<StockDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<Pedido>> GetAllPedidosAsync()
        {
            // Crea un nuevo contexto para la operación
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.Pedidos
                    .Include(p => p.Items!)
                        .ThenInclude(i => i.UbicacionProducto!)
                            .ThenInclude(up => up.Familia)
                    .ToListAsync();
            }
        }

        public async Task<Pedido?> GetPedidoByIdAsync(int id)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.Pedidos
                    .Include(p => p.Items!)
                        .ThenInclude(i => i.UbicacionProducto!)
                            .ThenInclude(up => up.Familia)
                    .Include(p => p.Items!)
                        .ThenInclude(i => i.TipoSoporte!)
                    .Include(p => p.Items!)
                        .ThenInclude(i => i.Repuestos!)
                            .ThenInclude(r => r.TipoRepuesto!)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }
        }

        public async Task<IEnumerable<Pedido>> GetAllPedidosWithDetailsAsync()
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.Pedidos
                    .Include(p => p.Items!)
                        .ThenInclude(i => i.TipoSoporte!)
                    .Include(p => p.Items!)
                        .ThenInclude(i => i.UbicacionProducto!)
                            .ThenInclude(up => up.Familia!) // IMPORTANTE: Incluir Familia
                    .Include(p => p.Items!)
                        .ThenInclude(i => i.Repuestos!)
                            .ThenInclude(r => r.TipoRepuesto!)
                    .ToListAsync();
            }
        }

        public async Task CreatePedidoAsync(Pedido pedido)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                // Esto es lo que necesitas para que funcione
                context.Entry(pedido).State = EntityState.Added;

                // Itera a través de todos los Items en el Pedido
                foreach (var item in pedido.Items)
                {
                    // Para cada Item, establece su estado como 'Added'
                    context.Entry(item).State = EntityState.Added;

                    // Para cada UbicacionProducto y TipoSoporte, verifica si tienen un Id
                    // Si lo tienen, significa que ya existen. Adjúntalos sin cambiarlos.
                    if (item.UbicacionProducto != null && item.UbicacionProducto.Id > 0)
                    {
                        context.Entry(item.UbicacionProducto).State = EntityState.Unchanged;
                    }

                    if (item.TipoSoporte != null && item.TipoSoporte.Id > 0)
                    {
                        context.Entry(item.TipoSoporte).State = EntityState.Unchanged;
                    }

                    // Itera a través de los Repuestos del Item
                    foreach (var repuesto in item.Repuestos)
                    {
                        // Para cada Repuesto, establece su estado como 'Added'
                        context.Entry(repuesto).State = EntityState.Added;

                        // Para cada TipoRepuesto, si tiene un Id, significa que ya existe.
                        // Adjúntalo sin cambiarlo para evitar el error.
                        if (repuesto.TipoRepuesto != null && repuesto.TipoRepuesto.Id > 0)
                        {
                            context.Entry(repuesto.TipoRepuesto).State = EntityState.Unchanged;
                        }
                    }
                }

                // Guarda todos los cambios en la base de datos de una sola vez
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdatePedidoAsync(Pedido pedido)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var existingPedido = await context.Pedidos
                    .Include(p => p.Items!)
                        .ThenInclude(i => i.Repuestos!)
                    .Include(p => p.Items!)
                        .ThenInclude(i => i.TipoSoporte!)
                    .Include(p => p.Items!)
                        .ThenInclude(i => i.UbicacionProducto!)
                            .ThenInclude(up => up.Familia)
                    .FirstOrDefaultAsync(p => p.Id == pedido.Id);

                if (existingPedido != null)
                {
                    context.Entry(existingPedido).CurrentValues.SetValues(pedido);

                    var itemsToRemove = existingPedido.Items!.Where(ei => !pedido.Items!.Any(pi => pi.Id == ei.Id)).ToList();
                    foreach (var item in itemsToRemove)
                    {
                        context.Items.Remove(item);
                    }

                    foreach (var pedidoItem in pedido.Items!)
                    {
                        var existingItem = existingPedido.Items.FirstOrDefault(ei => ei.Id == pedidoItem.Id);
                        if (existingItem == null)
                        {
                            existingPedido.Items.Add(pedidoItem);
                        }
                        else
                        {
                            context.Entry(existingItem).CurrentValues.SetValues(pedidoItem);

                            var repuestosToRemove = existingItem.Repuestos!.Where(er => !pedidoItem.Repuestos!.Any(pr => pr.Id == er.Id)).ToList();
                            foreach (var repuesto in repuestosToRemove)
                            {
                                context.Repuestos.Remove(repuesto);
                            }

                            foreach (var pedidoRepuesto in pedidoItem.Repuestos!)
                            {
                                var existingRepuesto = existingItem.Repuestos!.FirstOrDefault(er => er.Id == pedidoRepuesto.Id);
                                if (existingRepuesto == null)
                                {
                                    existingItem.Repuestos.Add(pedidoRepuesto);
                                }
                                else
                                {
                                    context.Entry(existingRepuesto).CurrentValues.SetValues(pedidoRepuesto);
                                }
                            }
                        }
                    }
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task DeletePedidoAsync(int id)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var pedido = await context.Pedidos.FindAsync(id);
                if (pedido != null)
                {
                    context.Pedidos.Remove(pedido);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
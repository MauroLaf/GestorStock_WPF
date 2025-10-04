using System.Collections.Generic;
using System.Threading.Tasks;
using GestorStock.Model.Entities;

namespace GestorStock.Services.Interfaces
{
    public interface IPedidoService
    {
        Task<List<Pedido>> GetAllAsync();
        Task<List<Pedido>> GetAllWithDetalleAsync(); // <-- NUEVO
        Task<Pedido?> GetByIdAsync(int id);
        Task<Pedido?> GetWithDetalleAsync(int id);

        Task<Pedido> CreateAsync(Pedido pedido);
        Task<Pedido> AddAsync(Pedido pedido);        // <-- ALIAS para compatibilidad
        Task<bool> UpdateAsync(Pedido pedido);
        Task<bool> DeleteAsync(int id);
    }
}

using System.Threading;
using System.Threading.Tasks;
using GestorStock.Model.Entities;

namespace GestorStock.Services.Interfaces
{
    public interface IPedidoService : ICrudService<Pedido, int>
    {
        Task<Pedido?> GetWithDetalleAsync(int id, CancellationToken ct = default);
    }
}

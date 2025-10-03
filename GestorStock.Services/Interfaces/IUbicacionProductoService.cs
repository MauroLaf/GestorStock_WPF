using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GestorStock.Model.Entities;

namespace GestorStock.Services.Interfaces
{
    public interface IUbicacionProductoService : ICrudService<UbicacionProducto, int>
    {
        Task<List<UbicacionProducto>> GetByFamiliaAsync(int familiaId, CancellationToken ct = default);
    }
}

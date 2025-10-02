using GestorStock.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorStock.Services.Interfaces
{
    public interface IUbicacionProductoService
    {
        Task<IEnumerable<UbicacionProducto>> GetAllUbicacionProductosAsync();
        Task<IEnumerable<UbicacionProducto>> GetUbicacionProductosByFamiliaIdAsync(int familiaId);
        Task<UbicacionProducto> CreateUbicacionProductoAsync(UbicacionProducto ubicacionProducto);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using GestorStock.Model.Entities;

namespace GestorStock.Services.Interfaces
{
    public interface IUbicacionProductoService
    {
        Task<List<UbicacionProducto>> GetAllAsync();
        Task<List<UbicacionProducto>> GetByFamiliaAsync(int familiaId);
        Task<UbicacionProducto?> GetByIdAsync(int id);
        Task<UbicacionProducto> CreateAsync(UbicacionProducto entity); // <- para botón +
        Task<bool> DeleteAsync(int id);
    }
}

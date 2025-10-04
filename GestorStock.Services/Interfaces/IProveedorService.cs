using System.Collections.Generic;
using System.Threading.Tasks;
using GestorStock.Model.Entities;

namespace GestorStock.Services.Interfaces
{
    public interface IProveedorService
    {
        Task<List<Proveedor>> GetAllAsync();
        Task<Proveedor?> GetByIdAsync(int id);
        Task<Proveedor> CreateAsync(Proveedor entity); // <- para botón +
        Task<bool> DeleteAsync(int id);
    }
}

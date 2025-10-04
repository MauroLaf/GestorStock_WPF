using System.Collections.Generic;
using System.Threading.Tasks;
using GestorStock.Model.Entities;

namespace GestorStock.Services.Interfaces
{
    public interface IRepuestoCatalogoService
    {
        Task<List<RepuestoCatalogo>> GetAllAsync();
        Task<RepuestoCatalogo?> GetByIdAsync(int id);
        Task<List<RepuestoCatalogo>> GetByFamiliaAsync(int familiaId);
        Task<bool> ExistsByNameAsync(string nombre, int? familiaId = null);

        Task<RepuestoCatalogo> CreateAsync(RepuestoCatalogo entity);
        Task<bool> UpdateAsync(RepuestoCatalogo entity);
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Devuelve un catálogo con ese nombre (y familia opcional). Si no existe, lo crea.
        /// Útil para el botón "+" de la UI.
        /// </summary>
        Task<RepuestoCatalogo> EnsureAsync(
            string nombre,
            int? familiaId = null,
            int? ubicacionProductoId = null,
            int? tipoSoporteId = null);
    }
}

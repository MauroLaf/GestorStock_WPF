using GestorStock.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorStock.Services.Interfaces
{
    public interface ITipoExplotacionService
    {
        // Cambia el nombre de este método para que coincida con tu clase de servicio
        Task<List<TipoExplotacion>> GetAllTipoExplotacionAsync();

        // El resto de los métodos que tengas
        Task<TipoExplotacion?> GetTipoExplotacionByIdAsync(int id);
        Task CreateTipoExplotacionAsync(TipoExplotacion tipo);
        Task UpdateTipoExplotacionAsync(TipoExplotacion tipo);
        Task DeleteTipoExplotacionAsync(int id);
    }
}
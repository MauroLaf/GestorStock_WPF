using GestorStock.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorStock.Services.Interfaces
{
    public interface ITipoExplotacionService
    {
        Task<List<TipoExplotacion>> GetAllTipoExplotacionAsync();
        Task<TipoExplotacion?> GetTipoExplotacionByIdAsync(int id);
        Task CreateTipoExplotacionAsync(TipoExplotacion tipo);
        Task UpdateTipoExplotacionAsync(TipoExplotacion tipo);
        Task DeleteTipoExplotacionAsync(int id);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using GestorStock.Model.Entities;

namespace GestorStock.Services.Interfaces
{
    public interface ITipoSoporteService
    {
        Task<List<TipoSoporte>> GetAllAsync();
        Task<TipoSoporte?> GetByIdAsync(int id);
        Task<TipoSoporte> CreateAsync(TipoSoporte entity); // <- para botón +
        Task<bool> DeleteAsync(int id);
    }
}

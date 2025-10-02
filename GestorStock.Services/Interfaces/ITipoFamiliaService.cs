using GestorStock.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorStock.Services.Interfaces
{
    public interface ITipoFamiliaService
    {
        Task<List<Familia>> GetAllTipoFamiliaAsync();
        Task<Familia?> GetTipoFamiliaByIdAsync(int id);
        Task CreateTipoFamiliaAsync(Familia tipo);
        Task UpdateTipoFamiliaAsync(Familia tipo);
        Task DeleteTipoFamiliaAsync(int id);
    }
}
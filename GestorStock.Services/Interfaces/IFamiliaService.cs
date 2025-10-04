using System.Collections.Generic;
using System.Threading.Tasks;
using GestorStock.Model.Entities;

namespace GestorStock.Services.Interfaces
{
    public interface IFamiliaService
    {
        Task<List<Familia>> GetAllAsync();
        Task<Familia?> GetByIdAsync(int id);
        Task<Familia> CreateAsync(Familia entity); // <- para botón +
        Task<bool> DeleteAsync(int id);
    }
}

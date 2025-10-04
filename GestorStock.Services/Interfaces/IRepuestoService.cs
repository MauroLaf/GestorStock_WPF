using System.Collections.Generic;
using System.Threading.Tasks;
using GestorStock.Model.Entities;

namespace GestorStock.Services.Interfaces
{
    public interface IRepuestoService
    {
        Task<List<Repuesto>> GetAllAsync();       // <- lo usa AddItemWindow
        Task<Repuesto?> GetByIdAsync(int id);

        Task<Repuesto> AddAsync(Repuesto entity); // <- faltaba
        Task<bool> UpdateAsync(Repuesto entity);
        Task<bool> DeleteAsync(int id);
    }
}

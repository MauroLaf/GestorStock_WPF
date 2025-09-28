using GestorStock.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorStock.Services.Interfaces
{
    public interface IRepuestoService
    {
        Task<IEnumerable<Repuesto>> GetAllRepuestosAsync();
        Task<Repuesto?> GetRepuestoByIdAsync(int id);
        Task CreateRepuestoAsync(Repuesto repuesto);
        Task UpdateRepuestoAsync(Repuesto repuesto);
        Task DeleteRepuestoAsync(int id);
        Task DescontarUnidadAsync(int id, int cantidad);
    }
}
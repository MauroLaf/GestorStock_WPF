using GestorStock.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorStock.Services.Interfaces
{
    public interface ITipoRepuestoService
    {
        Task<IEnumerable<TipoRepuesto>> GetAllTipoRepuestoAsync();
    }
}
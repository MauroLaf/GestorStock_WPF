using System.Collections.Generic;
using System.Threading.Tasks;
using GestorStock.Model.Enum;

namespace GestorStock.Services.Interfaces
{
    public interface ITipoRepuestoService
    {
        Task<List<TipoRepuestoEnum>> GetAllAsync();
    }
}

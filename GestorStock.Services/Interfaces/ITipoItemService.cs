using GestorStock.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorStock.Services.Interfaces
{
    public interface ITipoItemService
    {
        Task<List<TipoSoporte>> GetAllTipoItemAsync();
    }
}
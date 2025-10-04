using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GestorStock.Model.Enum;
using GestorStock.Services.Interfaces;

namespace GestorStock.Services.Implementations
{
    public class TipoRepuestoService : ITipoRepuestoService
    {
        public Task<List<TipoRepuestoEnum>> GetAllAsync() =>
            Task.FromResult(Enum.GetValues<TipoRepuestoEnum>().ToList());
    }
}

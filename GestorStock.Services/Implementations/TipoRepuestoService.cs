using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestorStock.Model.Enum;
using GestorStock.Services.Interfaces;

namespace GestorStock.Services.Implementations
{
    public class TipoRepuestoService : ITipoRepuestoService
    {
        public Task<List<TipoRepuestoEnum>> GetAllAsync()
            => Task.FromResult(Enum.GetValues<TipoRepuestoEnum>().ToList());
    }
}

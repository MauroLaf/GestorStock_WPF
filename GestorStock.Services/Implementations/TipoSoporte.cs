using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;

namespace GestorStock.Services.Implementations
{
    public class TipoSoporteService : BaseCrudService<TipoSoporte, int>, ITipoSoporteService
    { 
        public TipoSoporteService(StockDbContext ctx) : base(ctx) { } }
}

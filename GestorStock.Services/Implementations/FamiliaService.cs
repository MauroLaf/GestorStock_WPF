using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;

namespace GestorStock.Services.Implementations
{
    public class FamiliaService : BaseCrudService<Familia, int>, IFamiliaService
    {
        public FamiliaService(StockDbContext ctx) : base(ctx) { }
    }
}

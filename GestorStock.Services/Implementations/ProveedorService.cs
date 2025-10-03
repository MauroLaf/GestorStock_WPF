using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;

namespace GestorStock.Services.Implementations
{
    public class ProveedorService : BaseCrudService<Proveedor, int>, IProveedorService
    {
        public ProveedorService(StockDbContext ctx) : base(ctx) { }
    }
}

using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;

namespace GestorStock.Services.Implementations
{
    public class RepuestoService : BaseCrudService<Repuesto, int>, IRepuestoService
    { 
        public RepuestoService(StockDbContext ctx) : base(ctx) { } }
}

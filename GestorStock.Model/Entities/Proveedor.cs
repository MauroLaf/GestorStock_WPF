using System.Collections.Generic;

namespace GestorStock.Model.Entities
{
    public class Proveedor
    {
        public int ProveedorId { get; set; }
        public string? Nombre { get; set; } = string.Empty;
        public string? Telefono { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;

        public ICollection<Repuesto> Repuestos { get; set; } = new List<Repuesto>();
    }
}

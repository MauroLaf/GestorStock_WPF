using System.Collections.Generic;

namespace GestorStock.Model.Entities
{
    public class Proveedor
    {
        public int ProveedorId { get; set; }
        public string? Nombre { get; set; } = string.Empty;
        public string? Telefono { get; set; } = string.Empty; 
        public string? Email { get; set; } = string.Empty;

        // Relación con Items
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}

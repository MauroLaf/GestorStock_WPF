using System.Collections.Generic;
using System.Linq;

namespace GestorStock.Model.Entities
{
    public class UbicacionProducto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // Relación con la tabla de Explotaciones (Familias)
        public int FamiliaId { get; set; }
        public Familia? Familia { get; set; }

        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}

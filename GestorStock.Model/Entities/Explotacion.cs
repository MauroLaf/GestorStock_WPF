using System.Collections.Generic;

namespace GestorStock.Model.Entities
{
    public class Explotacion
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public ICollection<Item?> Items { get; set; } = new List<Item?>();

        public int TipoExplotacionId { get; set; }
        public TipoExplotacion? TipoExplotacion { get; set; }
    }
}
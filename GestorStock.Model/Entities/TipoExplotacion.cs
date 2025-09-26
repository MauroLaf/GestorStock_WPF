using System.Collections.Generic;

namespace GestorStock.Model.Entities
{
    public class TipoExplotacion
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
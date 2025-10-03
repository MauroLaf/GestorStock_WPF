using System.Collections.Generic;

namespace GestorStock.Model.Entities
{
    public class TipoSoporte
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Repuesto> Repuestos { get; set; } = new List<Repuesto>();
    }
}

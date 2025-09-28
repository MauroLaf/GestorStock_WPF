using System.Collections.Generic;

namespace GestorStock.Model.Entities
{
    public class TipoRepuesto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // NOTE: La colección de repuestos no debe ser anulable!!
        public ICollection<Repuesto> Repuestos { get; set; } = new List<Repuesto>();
    }
}
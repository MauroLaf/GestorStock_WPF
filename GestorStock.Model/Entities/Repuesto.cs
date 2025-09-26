using System.ComponentModel.DataAnnotations.Schema;

namespace GestorStock.Model.Entities
{
    public class Repuesto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public string Descripcion { get; set; } = string.Empty;

        // Nuevas propiedades para la relación con Item
        public int ItemId { get; set; }
        public Item? Item { get; set; }

        // Propiedades para la relación con TipoRepuesto
        public int TipoRepuestoId { get; set; }
        public TipoRepuesto? TipoRepuesto { get; set; }
    }
}
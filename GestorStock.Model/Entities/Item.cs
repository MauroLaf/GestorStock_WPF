using System.Collections.Generic;

namespace GestorStock.Model.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;

        // Propiedades para la relación con Pedido
        public int PedidoId { get; set; }
        public Pedido? Pedido { get; set; }
        public string NombreItem { get; set; } = string.Empty;

        // Propiedades para la relación con TipoExplotacion
        public int TipoExplotacionId { get; set; }
        public TipoExplotacion? TipoExplotacion { get; set; }

        // Propiedad para la relación con Repuesto
        public ICollection<Repuesto> Repuestos { get; set; } = new List<Repuesto>();

        // Propiedad para la relación con TipoItem
        public TipoItem? TipoItem { get; set; }
        public int TipoItemId { get; set; }
        public string RepuestosConcatenados => string.Join(", ", Repuestos.Select(r => $"{r.Nombre} ({r.Cantidad})"));

    }
}
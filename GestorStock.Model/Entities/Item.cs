using System.Collections.Generic;

namespace GestorStock.Model.Entities
{
    public class Item
    {
        // ID principal de la entidad
        public int ItemId { get; set; }

        // Relación con Pedido (un Pedido contiene muchos Items)
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; }

        // Propiedades de relaciones opcionales que son comunes
        public int? FamiliaId { get; set; }
        public Familia Familia { get; set; }

        public int? UbicacionId { get; set; }
        public UbicacionProducto UbicacionProducto { get; set; }

        public int? ProveedorId { get; set; }
        public Proveedor Proveedor { get; set; }

      
    }
}

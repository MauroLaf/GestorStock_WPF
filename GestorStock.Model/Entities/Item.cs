using GestorStock.Model.Entities;

public class Item
{
    public int ItemId { get; set; }

    // Relación con Pedido
    public int PedidoId { get; set; }
    public Pedido Pedido { get; set; }

    // Relación obligatoria con Familia
    public int FamiliaId { get; set; }
    public Familia Familia { get; set; }

    // Relación obligatoria con UbicacionProducto
    public int UbicacionProductoId { get; set; }
    public UbicacionProducto UbicacionProducto { get; set; }

    // Relación obligatoria con Proveedor
    public int ProveedorId { get; set; }
    public Proveedor Proveedor { get; set; }
}

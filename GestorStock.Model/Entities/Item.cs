namespace GestorStock.Model.Entities
{
    public class Item
    {
        public int Id { get; set; }

        //Propiedad de NombreProveedor
        //TODO: DEBO HACERLA LIST ADD/RM
        public string? NombreProveedor { get; set; }

        // Relación con Pedido
        public int PedidoId { get; set; }
        public Pedido? Pedido { get; set; }

        // Relación con Familia (lo llamaste TipoFamilia, pero es la misma clase)
        public int? FamiliaId { get; set; }
        public Familia? Familia { get; set; }

        // Relación con UbicacionProducto
        public int? UbicacionProductoId { get; set; }
        public UbicacionProducto? UbicacionProducto { get; set; }

        // Relación con TipoSoporte
        public int? TipoSoporteId { get; set; }
        public TipoSoporte? TipoSoporte { get; set; }

        // Relación con Repuestos
        public ICollection<Repuesto> Repuestos { get; set; } = new List<Repuesto>();
    }
}
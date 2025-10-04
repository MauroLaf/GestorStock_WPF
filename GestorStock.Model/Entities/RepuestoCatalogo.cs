namespace GestorStock.Model.Entities
{
    /// Catálogo de nombres de repuestos (para el ComboBox con +/-)
    public class RepuestoCatalogo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // Opcionalmente ligarlo a cabeceras “default”
        public int? FamiliaId { get; set; }
        public Familia? Familia { get; set; }

        public int? UbicacionProductoId { get; set; }
        public UbicacionProducto? UbicacionProducto { get; set; }

        public int? TipoSoporteId { get; set; }
        public TipoSoporte? TipoSoporte { get; set; }
    }
}

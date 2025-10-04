namespace GestorStock.Model.Entities
{
    public class Familia
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public ICollection<UbicacionProducto> Ubicaciones { get; set; } = new List<UbicacionProducto>();
        public ICollection<RepuestoCatalogo> Catalogo { get; set; } = new List<RepuestoCatalogo>();

        public ICollection<Repuesto> Repuestos { get; set; } = new List<Repuesto>();
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>(); // navegación inversa

    }
}

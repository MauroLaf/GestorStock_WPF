namespace GestorStock.Model.Entities
{
    public class Familia
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // Relación con UbicacionProducto
        public ICollection<UbicacionProducto> UbicacionProductos { get; set; } = new List<UbicacionProducto>();

        // Relación con Items
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}

namespace GestorStock.Model.Entities
{
    public class UbicacionProducto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public int FamiliaId { get; set; }
        public Familia Familia { get; set; } = null!;

        public ICollection<Repuesto> Repuestos { get; set; } = new List<Repuesto>();
    }
}

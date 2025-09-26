namespace GestorStock.API.ViewModels
{
    public class VistaItemPedido
    {
        // ID del Pedido (no del ítem)
        public int Id { get; set; }

        public string Tipo { get; set; } = string.Empty;
        public string Explotacion { get; set; } = string.Empty;
        public string NombreRepuesto { get; set; } = string.Empty;
        public string NombreCompletoItem { get; set; } = string.Empty;

        public int Cantidad { get; set; }
        public string TipoRepuesto { get; set; } = string.Empty;

        public string DescripcionPedido { get; set; } = string.Empty;
        public string DescripcionIncidencia { get; set; } = string.Empty;
        public DateTime FechaPedido { get; set; }
        public DateTime FechaIncidencia { get; set; }
        public bool Incidencia { get; set; } // Propiedad para el checkbox
    }
}
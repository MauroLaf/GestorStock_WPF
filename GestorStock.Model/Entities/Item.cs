using System.Collections.Generic;
using System.Linq;

namespace GestorStock.Model.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public Pedido? Pedido { get; set; }
        public string NombreItem { get; set; } = string.Empty;
        public int TipoItemId { get; set; }
        public TipoItem? TipoItem { get; set; }
        public int TipoExplotacionId { get; set; }
        public TipoExplotacion? TipoExplotacion { get; set; }

        public ICollection<Repuesto> Repuestos { get; set; } = new List<Repuesto>();

        public string RepuestosFormateados
        {
            get
            {
                if (Repuestos == null || !Repuestos.Any())
                {
                    return "Sin repuestos";
                }

                var repuestosStrings = Repuestos.Select(r => $"{r.Nombre} ({r.Cantidad}) - {r.TipoRepuesto?.Nombre}");
                return string.Join(", ", repuestosStrings);
            }
        }
    }
}
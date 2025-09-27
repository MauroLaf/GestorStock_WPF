using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorStock.Model.Entities
{
    public class Pedido
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string? Descripcion { get; set; } = string.Empty;
        public bool Incidencia { get; set; }
        public DateTime? FechaIncidencia { get; set; }
        public string? DescripcionIncidencia { get; set; } = string.Empty;

        // This is the one you should keep
        public DateTime FechaLlegada { get; set; }

        public ICollection<Item> Items { get; set; } = new List<Item>();

        // **PROPIEDAD CALCULADA: PEDIDO VENCIDO**
        [NotMapped]
        public bool EstaVencido => FechaLlegada.Date < DateTime.Today.Date;
    }
}
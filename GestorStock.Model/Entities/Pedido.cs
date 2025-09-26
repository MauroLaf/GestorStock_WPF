using System;
using System.Collections.Generic;

namespace GestorStock.Model.Entities
{
    public class Pedido
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty; // Inicializado
        public bool Incidencia { get; set; }
        public DateTime? FechaIncidencia { get; set; }
        public string? DescripcionIncidencia { get; set; } = string.Empty; // Acepta nulo

        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
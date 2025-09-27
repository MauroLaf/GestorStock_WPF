using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorStock.Model.Entities
{
    public class Pedido : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private DateTime? _fechaLlegada;

        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string? Descripcion { get; set; } = string.Empty;
        public bool Incidencia { get; set; }
        public DateTime? FechaIncidencia { get; set; }
        public string? DescripcionIncidencia { get; set; } = string.Empty;

        public DateTime? FechaLlegada
        {
            get => _fechaLlegada;
            set
            {
                if (_fechaLlegada != value)
                {
                    _fechaLlegada = value;
                    OnPropertyChanged(nameof(FechaLlegada));
                    // Notifica el cambio de la propiedad calculada
                    OnPropertyChanged(nameof(EstaVencido));
                }
            }
        }

        public ICollection<Item> Items { get; set; } = new List<Item>();

        [NotMapped]
        public bool EstaVencido => FechaLlegada.HasValue && FechaLlegada.Value.Date <= DateTime.Today.Date;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
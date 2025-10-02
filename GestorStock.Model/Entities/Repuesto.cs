using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorStock.Model.Entities
{
    public class Repuesto : INotifyPropertyChanged
    {
        // Evento que notifica a los suscriptores cuando una propiedad ha cambiado.
        public event PropertyChangedEventHandler? PropertyChanged;

        // Método para invocar el evento.
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Propiedades de la clase Repuesto
        public int Id { get; set; }

        private string _nombre = string.Empty;
        public string Nombre
        {
            get => _nombre;
            set
            {
                if (_nombre != value)
                {
                    _nombre = value;
                    OnPropertyChanged(nameof(Nombre));
                }
            }
        }

        private int _cantidad;
        public int Cantidad
        {
            get => _cantidad;
            set
            {
                if (_cantidad != value)
                {
                    _cantidad = value;
                    OnPropertyChanged(nameof(Cantidad));
                }
            }
        }

        public string Descripcion { get; set; } = string.Empty;

        private decimal _precio;
        public decimal Precio
        {
            get => _precio;
            set
            {
                if (_precio != value)
                {
                    _precio = value;
                    OnPropertyChanged(nameof(Precio));
                }
            }
        }

        // Propiedades de navegación para la relación con Item
        // ¡CAMBIO CLAVE AQUÍ! Hacemos ItemId anulable
        public int? ItemId { get; set; }
        public Item? Item { get; set; }

        // Propiedades para la relación con TipoRepuesto
        public int? TipoRepuestoId { get; set; }
        public TipoRepuesto? TipoRepuesto { get; set; }
    }
}
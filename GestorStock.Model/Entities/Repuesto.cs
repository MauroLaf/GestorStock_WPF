using System.ComponentModel;

namespace GestorStock.Model.Entities
{
    public class Repuesto : Item, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Propiedades propias de Repuesto
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

        private string _descripcion = string.Empty;
        public string Descripcion
        {
            get => _descripcion;
            set
            {
                if (_descripcion != value)
                {
                    _descripcion = value;
                    OnPropertyChanged(nameof(Descripcion));
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
        private TipoRepuestoEnum _tipoRepuesto;
        public TipoRepuestoEnum TipoRepuesto
        {
            get => _tipoRepuesto;
            set
            {
                if (_tipoRepuesto != value)
                {
                    _tipoRepuesto = value;
                    OnPropertyChanged(nameof(TipoRepuesto));
                }
            }
        }
    }
}

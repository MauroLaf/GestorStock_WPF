using GestorStock.Model.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

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
    public string? Factura { get; set; } = string.Empty;

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

    // Relación obligatoria con Familia
    public int FamiliaId { get; set; }
    public Familia Familia { get; set; }

    public ICollection<Item> Items { get; set; } = new List<Item>();

    [NotMapped]
    public bool EstaVencido => FechaLlegada.HasValue && FechaLlegada.Value.Date <= DateTime.Today.Date;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

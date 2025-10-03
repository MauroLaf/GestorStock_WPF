using GestorStock.Model.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

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
                OnPropertyChanged(nameof(EstaVencido));
            }
        }
    }

    // Relación obligatoria con Familia
    public int FamiliaId { get; set; }
    public Familia Familia { get; set; } = null!;

    // Detalle del pedido
    public ICollection<Repuesto> Repuestos { get; set; } = new List<Repuesto>();

    [NotMapped]
    public bool EstaVencido => FechaLlegada.HasValue && FechaLlegada.Value.Date <= DateTime.Today.Date;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

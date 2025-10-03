using GestorStock.Model.Entities;
using GestorStock.Model.Enum;
using System.ComponentModel;

public class Repuesto : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged(string p) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));

    // PK
    public int Id { get; set; }

    // ===== FKs que estaban en Item (ahora directamente en Repuesto) =====
    public int PedidoId { get; set; }
    public Pedido Pedido { get; set; } = null!;

    public int FamiliaId { get; set; }
    public Familia Familia { get; set; } = null!;

    public int UbicacionProductoId { get; set; }
    public UbicacionProducto UbicacionProducto { get; set; } = null!;

    public int ProveedorId { get; set; }
    public Proveedor Proveedor { get; set; } = null!;

    // Tipo de soporte: OBLIGATORIO
    public int TipoSoporteId { get; set; }
    public TipoSoporte TipoSoporte { get; set; } = null!;

    // ===== Propiedades propias de Repuesto (se mantienen) =====
    private string _nombre = string.Empty;
    public string Nombre { get => _nombre; set { if (_nombre != value) { _nombre = value; OnPropertyChanged(nameof(Nombre)); } } }

    private string _descripcion = string.Empty;
    public string Descripcion { get => _descripcion; set { if (_descripcion != value) { _descripcion = value; OnPropertyChanged(nameof(Descripcion)); } } }

    private int _cantidad;
    public int Cantidad { get => _cantidad; set { if (_cantidad != value) { _cantidad = value; OnPropertyChanged(nameof(Cantidad)); } } }

    private decimal _precio;
    public decimal Precio { get => _precio; set { if (_precio != value) { _precio = value; OnPropertyChanged(nameof(Precio)); } } }

    private TipoRepuestoEnum _tipoRepuesto;
    public TipoRepuestoEnum TipoRepuesto { get => _tipoRepuesto; set { if (_tipoRepuesto != value) { _tipoRepuesto = value; OnPropertyChanged(nameof(TipoRepuesto)); } } }
}

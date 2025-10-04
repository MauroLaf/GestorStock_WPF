using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GestorStock.Model.Entities;
using GestorStock.Model.Enum;
using GestorStock.Services.Interfaces;
using System.Runtime.Versioning;

namespace GestorStock.API.ViewModels
{
    [SupportedOSPlatform("windows")]
    public class CreatePedidoViewModel : BaseViewModel
    {
        private readonly IFamiliaService _familias;
        private readonly IUbicacionProductoService _ubicaciones;
        private readonly IProveedorService _proveedores;
        private readonly ITipoSoporteService _tiposSoporte;
        private readonly IPedidoService _pedidos;
        private readonly IRepuestoService _repuestos;

        public CreatePedidoViewModel(
            IFamiliaService familias, IUbicacionProductoService ubicaciones,
            IProveedorService proveedores, ITipoSoporteService tiposSoporte,
            IPedidoService pedidos, IRepuestoService repuestos)
        {
            _familias = familias;
            _ubicaciones = ubicaciones;
            _proveedores = proveedores;
            _tiposSoporte = tiposSoporte;
            _pedidos = pedidos;
            _repuestos = repuestos;

            AddRepuestoCommand = new RelayCommand(AddRepuesto, CanAddRepuesto);
            RemoveRepuestoCommand = new RelayCommand(RemoveRepuesto, () => SelectedRepuesto != null);
            SavePedidoCommand = new RelayCommand(async () => await SaveAsync(), CanSave);

            _ = LoadCombosAsync();
        }

        // ========= Campos de cabecera del Pedido =========
        private Familia? _familia;
        public Familia? SelectedFamilia
        {
            get => _familia;
            set { Set(ref _familia, value); _ = LoadUbicacionesAsync(); RaiseCanExecutes(); }
        }

        private UbicacionProducto? _ubicacion;
        public UbicacionProducto? SelectedUbicacion
        {
            get => _ubicacion;
            set { Set(ref _ubicacion, value); RaiseCanExecutes(); }
        }

        private Proveedor? _proveedor;
        public Proveedor? SelectedProveedor
        {
            get => _proveedor;
            set { Set(ref _proveedor, value); RaiseCanExecutes(); }
        }

        private TipoSoporte? _tipoSoporte;
        public TipoSoporte? SelectedTipoSoporte
        {
            get => _tipoSoporte;
            set { Set(ref _tipoSoporte, value); RaiseCanExecutes(); }
        }

        // Info del pedido
        private string? _descripcionPedido;
        public string? DescripcionPedido
        {
            get => _descripcionPedido;
            set { Set(ref _descripcionPedido, value); }
        }

        private DateTime? _fechaLlegada;
        public DateTime? FechaLlegada
        {
            get => _fechaLlegada;
            set { Set(ref _fechaLlegada, value); }
        }

        private bool _incidencia;
        public bool Incidencia
        {
            get => _incidencia;
            set { Set(ref _incidencia, value); }
        }

        private DateTime? _fechaIncidencia;
        public DateTime? FechaIncidencia
        {
            get => _fechaIncidencia;
            set { Set(ref _fechaIncidencia, value); }
        }

        private string? _descripcionIncidencia;
        public string? DescripcionIncidencia
        {
            get => _descripcionIncidencia;
            set { Set(ref _descripcionIncidencia, value); }
        }

        // ========= Detalle =========
        public ObservableCollection<Repuesto> Repuestos { get; } = new();

        private Repuesto? _selectedRepuesto;
        public Repuesto? SelectedRepuesto
        {
            get => _selectedRepuesto;
            set { Set(ref _selectedRepuesto, value); RemoveRepuestoCommand.RaiseCanExecuteChanged(); }
        }

        // ========= Campos para nueva línea =========
        private string? _nombre;
        public string? Nombre
        {
            get => _nombre;
            set { Set(ref _nombre, value); RaiseCanExecutes(); }
        }

        private string? _descripcionLinea;
        public string? Descripcion
        {
            get => _descripcionLinea;
            set { Set(ref _descripcionLinea, value); }
        }

        private int _cantidad = 1;
        public int Cantidad
        {
            get => _cantidad;
            set { Set(ref _cantidad, value); RaiseCanExecutes(); }
        }

        private decimal _precio;
        public decimal Precio
        {
            get => _precio;
            set { Set(ref _precio, value); }
        }

        public ObservableCollection<TipoRepuestoEnum> TiposRepuesto { get; } = new();
        private TipoRepuestoEnum _tipoRep = TipoRepuestoEnum.Original;
        public TipoRepuestoEnum SelectedTipoRepuesto
        {
            get => _tipoRep;
            set { Set(ref _tipoRep, value); }
        }

        // ========= Commands =========
        public RelayCommand AddRepuestoCommand { get; }
        public RelayCommand RemoveRepuestoCommand { get; }
        public RelayCommand SavePedidoCommand { get; }

        private void RaiseCanExecutes()
        {
            AddRepuestoCommand.RaiseCanExecuteChanged();
            SavePedidoCommand.RaiseCanExecuteChanged();
        }

        private bool CanAddRepuesto()
            => SelectedFamilia != null && SelectedUbicacion != null && SelectedProveedor != null
               && SelectedTipoSoporte != null && !string.IsNullOrWhiteSpace(Nombre) && Cantidad > 0;

        private void AddRepuesto()
        {
            var r = new Repuesto
            {
                Nombre = Nombre?.Trim() ?? "",
                Descripcion = Descripcion?.Trim() ?? "",
                Cantidad = Cantidad,
                Precio = Precio,
                TipoRepuesto = SelectedTipoRepuesto,

                // FKs del detalle (según tu modelo)
                FamiliaId = SelectedFamilia!.Id,
                UbicacionProductoId = SelectedUbicacion!.Id,
                ProveedorId = SelectedProveedor!.Id,        // <-- CORREGIDO
                TipoSoporteId = SelectedTipoSoporte!.Id
            };
            Repuestos.Add(r);

            // limpiar inputs de nueva línea
            Nombre = Descripcion = null;
            Cantidad = 1; Precio = 0;
            Raise(nameof(Nombre)); Raise(nameof(Descripcion)); Raise(nameof(Cantidad)); Raise(nameof(Precio));
            RaiseCanExecutes();
        }

        private void RemoveRepuesto()
        {
            if (SelectedRepuesto != null) Repuestos.Remove(SelectedRepuesto);
            RaiseCanExecutes();
        }

        private bool CanSave() =>
            SelectedFamilia != null && SelectedUbicacion != null &&
            SelectedProveedor != null && SelectedTipoSoporte != null &&
            Repuestos.Any();

        private async Task SaveAsync()
        {
            var pedido = new Pedido
            {
                FechaCreacion = DateTime.Now,
                Descripcion = DescripcionPedido?.Trim(),
                Incidencia = Incidencia,
                FechaIncidencia = FechaIncidencia,
                DescripcionIncidencia = DescripcionIncidencia?.Trim(),
                FechaLlegada = FechaLlegada,
                FamiliaId = SelectedFamilia!.Id
            };

            // líneas
            foreach (var r in Repuestos) pedido.Repuestos.Add(r);

            // Guardar (cascada)
            await _pedidos.CreateAsync(pedido); // o await _pedidos.AddAsync(pedido);

            // limpiar después de guardar si quieres
            Repuestos.Clear();
        }

        private async Task LoadCombosAsync()
        {
            Familias.Clear();
            foreach (var f in await _familias.GetAllAsync()) Familias.Add(f);

            Proveedores.Clear();
            foreach (var p in await _proveedores.GetAllAsync()) Proveedores.Add(p);

            TiposSoporte.Clear();
            foreach (var t in await _tiposSoporte.GetAllAsync()) TiposSoporte.Add(t);
        }

        private async Task LoadUbicacionesAsync()
        {
            Ubicaciones.Clear();
            if (SelectedFamilia != null)
            {
                var lista = await _ubicaciones.GetByFamiliaAsync(SelectedFamilia.Id);
                foreach (var u in lista) Ubicaciones.Add(u);
            }
            RaiseCanExecutes();
        }

        // Combos expuestos
        public ObservableCollection<Familia> Familias { get; } = new();
        public ObservableCollection<UbicacionProducto> Ubicaciones { get; } = new();
        public ObservableCollection<Proveedor> Proveedores { get; } = new();
        public ObservableCollection<TipoSoporte> TiposSoporte { get; } = new();
    }
}

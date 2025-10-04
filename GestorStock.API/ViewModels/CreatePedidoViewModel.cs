using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GestorStock.API.ViewModels;
using GestorStock.Model.Entities;
using GestorStock.Model.Enum;
using GestorStock.Services.Interfaces;

namespace GestorStock.API.ViewModels
{
    public class CreatePedidoViewModel : BaseViewModel
    {
        private readonly IFamiliaService _familias;
        private readonly IUbicacionProductoService _ubicaciones;
        private readonly IProveedorService _proveedores;
        private readonly ITipoSoporteService _tiposSoporte;
        private readonly ITipoRepuestoService _tiposRepuesto;
        private readonly IPedidoService _pedidos;
        private readonly IRepuestoService _repuestos;

        public CreatePedidoViewModel(
            IFamiliaService familias, IUbicacionProductoService ubicaciones,
            IProveedorService proveedores, ITipoSoporteService tiposSoporte,
            ITipoRepuestoService tiposRepuesto,
            IPedidoService pedidos, IRepuestoService repuestos)
        {
            _familias = familias;
            _ubicaciones = ubicaciones;
            _proveedores = proveedores;
            _tiposSoporte = tiposSoporte;
            _tiposRepuesto = tiposRepuesto;
            _pedidos = pedidos;
            _repuestos = repuestos;

            AddRepuestoCommand = new RelayCommand(AddRepuesto, CanAddRepuesto);
            RemoveRepuestoCommand = new RelayCommand(RemoveRepuesto, () => SelectedRepuesto != null);
            SavePedidoCommand = new RelayCommand(async () => await SaveAsync(), CanSave);

            _ = LoadCombosAsync();
        }

        // Combos
        public ObservableCollection<Familia> Familias { get; } = new();
        public ObservableCollection<UbicacionProducto> Ubicaciones { get; } = new();
        public ObservableCollection<Proveedor> Proveedores { get; } = new();
        public ObservableCollection<TipoSoporte> TiposSoporte { get; } = new();
        public ObservableCollection<TipoRepuestoEnum> TiposRepuesto { get; } = new();

        private Familia? _familia;
        public Familia? SelectedFamilia
        {
            get => _familia;
            set { Set(ref _familia, value); _ = LoadUbicacionesAsync(); }
        }
        public UbicacionProducto? SelectedUbicacion { get; set; }
        public Proveedor? SelectedProveedor { get; set; }
        public TipoSoporte? SelectedTipoSoporte { get; set; }

        // Detalle
        public ObservableCollection<Repuesto> Repuestos { get; } = new();

        private Repuesto? _selectedRepuesto;
        public Repuesto? SelectedRepuesto
        {
            get => _selectedRepuesto;
            set { Set(ref _selectedRepuesto, value); RemoveRepuestoCommand.RaiseCanExecuteChanged(); }
        }

        // Campos para nueva línea
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int Cantidad { get; set; } = 1;
        public decimal Precio { get; set; }
        public TipoRepuestoEnum SelectedTipoRepuesto { get; set; } = TipoRepuestoEnum.Original;

        // Commands
        public RelayCommand AddRepuestoCommand { get; }
        public RelayCommand RemoveRepuestoCommand { get; }
        public RelayCommand SavePedidoCommand { get; }

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
                ProveedorId = SelectedProveedor!.ProveedorId,
                TipoSoporteId = SelectedTipoSoporte!.Id
            };
            Repuestos.Add(r);

            // limpiar inputs de nueva línea
            Nombre = Descripcion = null;
            Cantidad = 1; Precio = 0;
            Raise(nameof(Nombre)); Raise(nameof(Descripcion)); Raise(nameof(Cantidad)); Raise(nameof(Precio));
            AddRepuestoCommand.RaiseCanExecuteChanged();
            SavePedidoCommand.RaiseCanExecuteChanged();
        }

        private void RemoveRepuesto()
        {
            if (SelectedRepuesto != null) Repuestos.Remove(SelectedRepuesto);
            SavePedidoCommand.RaiseCanExecuteChanged();
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
                FamiliaId = SelectedFamilia!.Id
            };

            pedido.Repuestos = Repuestos.ToList();
            await _pedidos.AddAsync(pedido); // guardará cascada los repuestos (FK PedidoId se setea al salvar)

            // limpiar todo tras guardar
            Repuestos.Clear();
        }

        private async Task LoadCombosAsync()
        {
            Familias.Clear(); (await _familias.GetAllAsync()).ForEach(Familias.Add);
            Proveedores.Clear(); (await _proveedores.GetAllAsync()).ForEach(Proveedores.Add);
            TiposSoporte.Clear(); (await _tiposSoporte.GetAllAsync()).ForEach(TiposSoporte.Add);
            TiposRepuesto.Clear(); (await _tiposRepuesto.GetAllAsync()).ForEach(TiposRepuesto.Add);
        }

        private async Task LoadUbicacionesAsync()
        {
            Ubicaciones.Clear();
            if (SelectedFamilia != null)
                (await _ubicaciones.GetByFamiliaAsync(SelectedFamilia.Id)).ForEach(Ubicaciones.Add);
            AddRepuestoCommand.RaiseCanExecuteChanged();
            SavePedidoCommand.RaiseCanExecuteChanged();
        }
    }
}

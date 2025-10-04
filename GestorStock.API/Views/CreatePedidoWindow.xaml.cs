using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GestorStock.API.Views
{
    public partial class CreatePedidoWindow : Window
    {
        private readonly IPedidoService _pedidoService;
        private readonly IFamiliaService _familiaService;
        private readonly ITipoSoporteService _tipoSoporteService;
        private readonly IProveedorService _proveedorService;
        private readonly IUbicacionProductoService _ubicacionProductoService;
        private readonly IRepuestoCatalogoService _catalogoService;
        private readonly IServiceProvider _sp;

        private readonly ObservableCollection<Repuesto> _detalle = new();
        private Pedido? _pedidoEditar;

        public CreatePedidoWindow(
            IPedidoService pedidoService,
            IFamiliaService familiaService,
            ITipoSoporteService tipoSoporteService,
            IProveedorService proveedorService,
            IUbicacionProductoService ubicacionProductoService,
            IRepuestoCatalogoService catalogoService,
            IServiceProvider sp,
            Pedido? pedidoAEditar = null)
        {
            InitializeComponent();

            _pedidoService = pedidoService;
            _familiaService = familiaService;
            _tipoSoporteService = tipoSoporteService;
            _proveedorService = proveedorService;
            _ubicacionProductoService = ubicacionProductoService;
            _catalogoService = catalogoService;
            _sp = sp;
            _pedidoEditar = pedidoAEditar;

            dgRepuestos.ItemsSource = _detalle;

            Loaded += async (_, __) => await CreatePedidoWindow_Loaded();
        }

        private async Task CreatePedidoWindow_Loaded()
        {
            cmbFamilia.ItemsSource = await _familiaService.GetAllAsync();
            cmbTipoSoporte.ItemsSource = await _tipoSoporteService.GetAllAsync();
            cmbProveedor.ItemsSource = await _proveedorService.GetAllAsync();

            if (_pedidoEditar != null)
            {
                // Cabecera según el primer detalle (tu entidad Pedido tiene FamiliaId obligatorio)
                var fam = (_pedidoEditar.FamiliaId > 0) ? (await _familiaService.GetAllAsync())
                             .FirstOrDefault(f => f.Id == _pedidoEditar.FamiliaId) : null;
                if (fam != null)
                {
                    cmbFamilia.SelectedItem = fam;
                    cmbUbicacion.ItemsSource = await _ubicacionProductoService.GetByFamiliaAsync(fam.Id);
                }

                // Rellenar detalle
                foreach (var r in _pedidoEditar.Repuestos)
                    _detalle.Add(r);
            }
        }

        private async void cmbFamilia_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cmbFamilia.SelectedItem is Familia fam)
                cmbUbicacion.ItemsSource = await _ubicacionProductoService.GetByFamiliaAsync(fam.Id);
            else
                cmbUbicacion.ItemsSource = null;
        }

        // ====== Botonera detalle ======
        private void BtnAgregarLinea_Click(object sender, RoutedEventArgs e)
        {
            var win = _sp.GetRequiredService<AddItemWindow>();
            win.Owner = this;

            // prefijar cabecera
            win.PrefijarCabecera(
                cmbFamilia.SelectedItem as Familia,
                cmbUbicacion.SelectedItem as UbicacionProducto,
                cmbProveedor.SelectedItem as Proveedor,
                cmbTipoSoporte.SelectedItem as TipoSoporte);

            if (win.ShowDialog() == true && win.RepuestosCreados.Count > 0)
            {
                foreach (var r in win.RepuestosCreados)
                {
                    // fuerza coherencia con la cabecera
                    if (cmbFamilia.SelectedItem is Familia f) { r.FamiliaId = f.Id; r.Familia = f; }
                    if (cmbUbicacion.SelectedItem is UbicacionProducto u) { r.UbicacionProductoId = u.Id; r.UbicacionProducto = u; }
                    if (cmbProveedor.SelectedItem is Proveedor p) { r.ProveedorId = p.Id; r.Proveedor = p; }
                    if (cmbTipoSoporte.SelectedItem is TipoSoporte s) { r.TipoSoporteId = s.Id; r.TipoSoporte = s; }

                    _detalle.Add(r);
                }
            }
        }

        private void BtnEditarLinea_Click(object sender, RoutedEventArgs e)
        {
            if (dgRepuestos.SelectedItem is not Repuesto sel) { MessageBox.Show("Selecciona una línea."); return; }

            var win = _sp.GetRequiredService<AddItemWindow>();
            win.Owner = this;

            // Prefija cabecera actual
            win.PrefijarCabecera(
                cmbFamilia.SelectedItem as Familia,
                cmbUbicacion.SelectedItem as UbicacionProducto,
                cmbProveedor.SelectedItem as Proveedor,
                cmbTipoSoporte.SelectedItem as TipoSoporte);

            // Carga el repuesto seleccionado
            win.CargarParaEdicion(sel);

            if (win.ShowDialog() == true && win.RepuestosCreados.Count == 1)
            {
                var editado = win.RepuestosCreados.First();
                var idx = _detalle.IndexOf(sel);
                if (idx >= 0) _detalle[idx] = editado;
            }
        }

        private void BtnQuitarLinea_Click(object sender, RoutedEventArgs e)
        {
            if (dgRepuestos.SelectedItem is Repuesto sel) _detalle.Remove(sel);
        }

        // ====== Guardar ======
        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbFamilia.SelectedItem is not Familia fam
                || cmbUbicacion.SelectedItem is not UbicacionProducto ubi
                || cmbProveedor.SelectedItem is not Proveedor prov
                || cmbTipoSoporte.SelectedItem is not TipoSoporte sop)
            {
                MessageBox.Show("Completa la cabecera del pedido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Asigna cabecera a todas las líneas
            foreach (var r in _detalle)
            {
                r.FamiliaId = fam.Id; r.Familia = fam;
                r.UbicacionProductoId = ubi.Id; r.UbicacionProducto = ubi;
                r.ProveedorId = prov.Id; r.Proveedor = prov;
                r.TipoSoporteId = sop.Id; r.TipoSoporte = sop;
            }

            if (_pedidoEditar == null)
            {
                var nuevo = new Pedido
                {
                    FechaCreacion = System.DateTime.Now,
                    Descripcion = "Pedido creado desde CreatePedidoWindow",
                    FamiliaId = fam.Id,
                    Familia = fam,
                    Repuestos = _detalle.ToList()
                };
                await _pedidoService.CreateAsync(nuevo);
            }
            else
            {
                _pedidoEditar.FamiliaId = fam.Id;
                _pedidoEditar.Familia = fam;
                _pedidoEditar.Repuestos = _detalle.ToList();
                await _pedidoService.UpdateAsync(_pedidoEditar);
            }

            DialogResult = true;
            Close();
        }
    }
}

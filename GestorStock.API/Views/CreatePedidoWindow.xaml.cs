using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows; // Necesario para MessageBox
using GestorStock.Model.Entities;
using GestorStock.Model.Enum;
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
            Pedido? pedidoEditar)
        {
            InitializeComponent();
            _pedidoService = pedidoService;
            _familiaService = familiaService;
            _tipoSoporteService = tipoSoporteService;
            _proveedorService = proveedorService;
            _ubicacionProductoService = ubicacionProductoService;
            _catalogoService = catalogoService;
            _sp = sp;
            _pedidoEditar = pedidoEditar;

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
                txtDescripcionPedido.Text = _pedidoEditar.Descripcion;
                dpFechaLlegada.SelectedDate = _pedidoEditar.FechaLlegada;
                chkIncidencia.IsChecked = _pedidoEditar.Incidencia;
                dpFechaIncidencia.SelectedDate = _pedidoEditar.FechaIncidencia;
                txtDescripcionIncidencia.Text = _pedidoEditar.DescripcionIncidencia;

                // Cabecera según familia
                var fam = (_pedidoEditar.FamiliaId > 0)
                    ? (await _familiaService.GetAllAsync()).FirstOrDefault(f => f.Id == _pedidoEditar.FamiliaId)
                    : null;

                if (fam != null)
                {
                    cmbFamilia.SelectedItem = fam;
                    cmbUbicacion.ItemsSource = await _ubicacionProductoService.GetByFamiliaAsync(fam.Id);
                }

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

        // ====== Detalle ======
        private void BtnAgregarLinea_Click(object sender, RoutedEventArgs e)
        {
            var win = _sp.GetRequiredService<AddItemWindow>();
            win.Owner = this;

            win.PrefijarCabecera(
                cmbFamilia.SelectedItem as Familia,
                cmbUbicacion.SelectedItem as UbicacionProducto,
                cmbProveedor.SelectedItem as Proveedor,
                cmbTipoSoporte.SelectedItem as TipoSoporte);

            if (win.ShowDialog() == true && win.RepuestosCreados.Count > 0)
            {
                foreach (var r in win.RepuestosCreados) _detalle.Add(r);
            }
        }

        private void BtnEditarLinea_Click(object sender, RoutedEventArgs e)
        {
            if (dgRepuestos.SelectedItem is not Repuesto sel)
            {
                MessageBox.Show("Selecciona una línea.");
                return;
            }

            var win = _sp.GetRequiredService<AddItemWindow>();
            win.Owner = this;

            win.PrefijarCabecera(
                cmbFamilia.SelectedItem as Familia,
                cmbUbicacion.SelectedItem as UbicacionProducto,
                cmbProveedor.SelectedItem as Proveedor,
                cmbTipoSoporte.SelectedItem as TipoSoporte);

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

        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => Close();

        // ====== GUARDAR ======
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

            // Construir un Pedido con SOLO FKs (para evitar duplicados / insert de entidades existentes)
            var nuevo = new Pedido
            {
                FechaCreacion = DateTime.Now,
                Descripcion = txtDescripcionPedido.Text?.Trim(),
                Incidencia = chkIncidencia.IsChecked == true,
                FechaIncidencia = dpFechaIncidencia.SelectedDate,
                DescripcionIncidencia = txtDescripcionIncidencia.Text?.Trim(),
                FechaLlegada = dpFechaLlegada.SelectedDate,
                FamiliaId = fam.Id,
                Repuestos = _detalle.Select(r => new Repuesto
                {
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion,
                    Cantidad = r.Cantidad,
                    Precio = r.Precio,
                    TipoRepuesto = r.TipoRepuesto,
                    FamiliaId = fam.Id,
                    UbicacionProductoId = ubi.Id,
                    ProveedorId = prov.Id,
                    TipoSoporteId = sop.Id
                }).ToList()
            };

            if (_pedidoEditar == null)
            {
                await _pedidoService.CreateAsync(nuevo);

                // 🔔 ALERTA DE CREACIÓN EXITOSA
                MessageBox.Show(
                    "El pedido se ha creado correctamente.",
                    "Creación Exitosa",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            else
            {
                // Update: sustituimos las líneas por las nuevas del formulario
                _pedidoEditar.FechaCreacion = nuevo.FechaCreacion;
                _pedidoEditar.Descripcion = nuevo.Descripcion;
                _pedidoEditar.Incidencia = nuevo.Incidencia;
                _pedidoEditar.FechaIncidencia = nuevo.FechaIncidencia;
                _pedidoEditar.DescripcionIncidencia = nuevo.DescripcionIncidencia;
                _pedidoEditar.FechaLlegada = nuevo.FechaLlegada;
                _pedidoEditar.FamiliaId = nuevo.FamiliaId;
                _pedidoEditar.Repuestos = nuevo.Repuestos;
                await _pedidoService.UpdateAsync(_pedidoEditar);

                // 🔔 ALERTA DE EDICIÓN EXITOSA (Opcional)
                MessageBox.Show(
                    "El pedido se ha actualizado correctamente.",
                    "Actualización Exitosa",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }

            DialogResult = true;
            Close();
        }
    }
}
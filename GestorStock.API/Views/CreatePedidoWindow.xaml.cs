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
        private readonly IPedidoService _pedidos;
        private readonly IRepuestoService _repuestos;
        private readonly IFamiliaService _familias;
        private readonly IUbicacionProductoService _ubicaciones;
        private readonly IProveedorService _proveedores;
        private readonly ITipoSoporteService _tiposSoporte;
        private readonly IServiceProvider _sp;

        // Se asigna desde MainWindow (opción A)
        public Pedido? PedidoEditar { get; set; }

        private readonly ObservableCollection<Repuesto> _detalle = new();

        public CreatePedidoWindow(IPedidoService pedidos,
                                  IRepuestoService repuestos,
                                  IFamiliaService familias,
                                  ITipoSoporteService tiposSoporte,
                                  IProveedorService proveedores,
                                  IUbicacionProductoService ubicaciones,
                                  IServiceProvider sp)
        {
            InitializeComponent();
            _pedidos = pedidos; _repuestos = repuestos; _familias = familias;
            _tiposSoporte = tiposSoporte; _proveedores = proveedores; _ubicaciones = ubicaciones;
            _sp = sp;

            dgRepuestos.ItemsSource = _detalle;
            Loaded += CreatePedidoWindow_Loaded;
        }

        private async void CreatePedidoWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Cargar combos de cabecera
            cmbFamilia.ItemsSource = await _familias.GetAllAsync();
            cmbProveedor.ItemsSource = await _proveedores.GetAllAsync();
            cmbTipoSoporte.ItemsSource = await _tiposSoporte.GetAllAsync();

            if (PedidoEditar != null)
            {
                // Seleccionar familia del pedido
                var familias = (cmbFamilia.ItemsSource as System.Collections.Generic.IEnumerable<Familia>)!;
                cmbFamilia.SelectedItem = familias.FirstOrDefault(f => f.Id == PedidoEditar.FamiliaId);

                // Cargar ubicaciones según familia seleccionada
                await CargarUbicacionesAsync();

                // Cargar detalle desde BD (por si viene desactualizado en memoria)
                var ped = await _pedidos.GetWithDetalleAsync(PedidoEditar.Id);
                _detalle.Clear();
                if (ped?.Repuestos != null)
                {
                    foreach (var r in ped.Repuestos)
                        _detalle.Add(r);
                }
            }
        }

        private async Task CargarUbicacionesAsync()
        {
            cmbUbicacion.ItemsSource = null;
            if (cmbFamilia.SelectedItem is Familia f)
                cmbUbicacion.ItemsSource = await _ubicaciones.GetByFamiliaAsync(f.Id);
        }

        private async void cmbFamilia_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
            => await CargarUbicacionesAsync();

        private void BtnAgregarLinea_Click(object sender, RoutedEventArgs e)
        {
            var win = _sp.GetRequiredService<AddItemWindow>();
            win.Owner = this;

            if (win.ShowDialog() == true && win.RepuestoCreado != null)
            {
                // Alinear con cabecera si corresponde
                if (cmbFamilia.SelectedItem is Familia fam) win.RepuestoCreado.FamiliaId = fam.Id;
                if (cmbUbicacion.SelectedItem is UbicacionProducto ubi) win.RepuestoCreado.UbicacionProductoId = ubi.Id;

                _detalle.Add(win.RepuestoCreado);
            }
        }

        private void BtnQuitarLinea_Click(object sender, RoutedEventArgs e)
        {
            if (dgRepuestos.SelectedItem is Repuesto sel)
                _detalle.Remove(sel);
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbFamilia.SelectedItem is not Familia fam ||
                cmbUbicacion.SelectedItem is not UbicacionProducto ubi ||
                cmbProveedor.SelectedItem is not Proveedor prov ||
                cmbTipoSoporte.SelectedItem is not TipoSoporte sop)
            {
                MessageBox.Show("Completa la cabecera del pedido.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (PedidoEditar == null)
            {
                // Crear
                var nuevo = new Pedido
                {
                    FechaCreacion = System.DateTime.Now,
                    FamiliaId = fam.Id,
                    Descripcion = "Pedido creado desde CreatePedidoWindow"
                };

                foreach (var r in _detalle)
                {
                    r.FamiliaId = fam.Id;
                    if (r.UbicacionProductoId == 0) r.UbicacionProductoId = ubi.Id;
                    if (r.ProveedorId == 0) r.ProveedorId = prov.ProveedorId;
                    if (r.TipoSoporteId == 0) r.TipoSoporteId = sop.Id;
                }

                nuevo.Repuestos = _detalle.ToList();
                await _pedidos.AddAsync(nuevo);
            }
            else
            {
                // Editar: sincroniza detalle de forma simple
                var ped = await _pedidos.GetWithDetalleAsync(PedidoEditar.Id);
                if (ped == null)
                {
                    MessageBox.Show("No se encontró el pedido.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ped.FamiliaId = fam.Id;

                // Borra líneas existentes y vuelve a insertar
                var existentes = ped.Repuestos?.ToList() ?? new();
                foreach (var r in existentes)
                    await _repuestos.DeleteAsync(r.Id);

                foreach (var r in _detalle)
                {
                    r.PedidoId = ped.Id;
                    r.FamiliaId = fam.Id;
                    if (r.UbicacionProductoId == 0) r.UbicacionProductoId = ubi.Id;
                    if (r.ProveedorId == 0) r.ProveedorId = prov.ProveedorId;
                    if (r.TipoSoporteId == 0) r.TipoSoporteId = sop.Id;

                    await _repuestos.AddAsync(r);
                }

                await _pedidos.UpdateAsync(ped);
            }

            DialogResult = true;
            Close();
        }
    }
}

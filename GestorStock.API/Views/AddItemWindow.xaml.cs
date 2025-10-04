using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GestorStock.Model.Entities;
using GestorStock.Model.Enum;
using GestorStock.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GestorStock.API.Views
{
    public partial class AddItemWindow : Window
    {
        private readonly IProveedorService _proveedorService;
        private readonly IFamiliaService _familiaService;
        private readonly IUbicacionProductoService _ubicacionService;
        private readonly ITipoSoporteService _tipoSoporteService;
        private readonly IRepuestoCatalogoService _catalogoService;
        private readonly IServiceProvider _sp;

        private readonly ObservableCollection<Repuesto> _buffer = new();
        private int? _editIndex = null;

        public ReadOnlyObservableCollection<Repuesto> RepuestosCreados => new(_buffer);

        public AddItemWindow(
            IProveedorService proveedorService,
            IFamiliaService familiaService,
            IUbicacionProductoService ubicacionService,
            ITipoSoporteService tipoSoporteService,
            IRepuestoCatalogoService catalogoService,
            IServiceProvider sp)
        {
            InitializeComponent();
            _proveedorService = proveedorService;
            _familiaService = familiaService;
            _ubicacionService = ubicacionService;
            _tipoSoporteService = tipoSoporteService;
            _catalogoService = catalogoService;
            _sp = sp;

            dgTemp.ItemsSource = _buffer;

            Loaded += async (_, __) => await CargarCombosAsync();
        }

        #region Carga combos / dependencias
        private async Task CargarCombosAsync()
        {
            cmbProveedor.ItemsSource = await _proveedorService.GetAllAsync();
            cmbFamilia.ItemsSource = await _familiaService.GetAllAsync();
            cmbTipoSoporte.ItemsSource = await _tipoSoporteService.GetAllAsync();
            cmbRepuestoCat.ItemsSource = await _catalogoService.GetAllAsync();
        }

        private async void cmbFamilia_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cmbFamilia.SelectedItem is Familia fam)
                cmbUbicacion.ItemsSource = await _ubicacionService.GetByFamiliaAsync(fam.Id);
            else
                cmbUbicacion.ItemsSource = null;
        }
        #endregion

        #region "+" y "-" de combos (InputBox rápido)
        private static string? Input(string titulo, string msg)
        {
            return Microsoft.VisualBasic.Interaction.InputBox(msg, titulo, "");
        }

        private async void btnAddProveedor_Click(object sender, RoutedEventArgs e)
        {
            var txt = Input("Nuevo proveedor", "Nombre:");
            if (string.IsNullOrWhiteSpace(txt)) return;
            var nuevo = await _proveedorService.CreateAsync(new Proveedor { Nombre = txt.Trim() });
            await CargarCombosAsync();
            cmbProveedor.SelectedItem = nuevo;
        }
        private async void btnDelProveedor_Click(object sender, RoutedEventArgs e)
        {
            if (cmbProveedor.SelectedItem is Proveedor p) { await _proveedorService.DeleteAsync(p.Id); await CargarCombosAsync(); }
        }

        private async void btnAddFamilia_Click(object sender, RoutedEventArgs e)
        {
            var txt = Input("Nueva familia", "Nombre:");
            if (string.IsNullOrWhiteSpace(txt)) return;
            var nuevo = await _familiaService.CreateAsync(new Familia { Nombre = txt.Trim() });
            await CargarCombosAsync();
            cmbFamilia.SelectedItem = nuevo;
        }
        private async void btnDelFamilia_Click(object sender, RoutedEventArgs e)
        {
            if (cmbFamilia.SelectedItem is Familia f) { await _familiaService.DeleteAsync(f.Id); await CargarCombosAsync(); }
        }

        private async void btnAddUbicacion_Click(object sender, RoutedEventArgs e)
        {
            if (cmbFamilia.SelectedItem is not Familia f) { MessageBox.Show("Primero selecciona una familia."); return; }
            var txt = Input("Nueva ubicación", "Nombre:");
            if (string.IsNullOrWhiteSpace(txt)) return;
            var nuevo = await _ubicacionService.CreateAsync(new UbicacionProducto { Nombre = txt.Trim(), FamiliaId = f.Id });
            cmbUbicacion.ItemsSource = await _ubicacionService.GetByFamiliaAsync(f.Id);
            cmbUbicacion.SelectedItem = nuevo;
        }
        private async void btnDelUbicacion_Click(object sender, RoutedEventArgs e)
        {
            if (cmbUbicacion.SelectedItem is UbicacionProducto u) { await _ubicacionService.DeleteAsync(u.Id); if (cmbFamilia.SelectedItem is Familia f) cmbUbicacion.ItemsSource = await _ubicacionService.GetByFamiliaAsync(f.Id); }
        }

        private async void btnAddSoporte_Click(object sender, RoutedEventArgs e)
        {
            var txt = Input("Nuevo tipo de soporte", "Nombre:");
            if (string.IsNullOrWhiteSpace(txt)) return;
            var nuevo = await _tipoSoporteService.CreateAsync(new TipoSoporte { Nombre = txt.Trim() });
            await CargarCombosAsync();
            cmbTipoSoporte.SelectedItem = nuevo;
        }
        private async void btnDelSoporte_Click(object sender, RoutedEventArgs e)
        {
            if (cmbTipoSoporte.SelectedItem is TipoSoporte s) { await _tipoSoporteService.DeleteAsync(s.Id); await CargarCombosAsync(); }
        }

        private async void btnAddRepuestoCat_Click(object sender, RoutedEventArgs e)
        {
            var txt = Input("Nuevo repuesto (catálogo)", "Nombre:");
            if (string.IsNullOrWhiteSpace(txt)) return;

            var nuevo = await _catalogoService.CreateAsync(new RepuestoCatalogo
            {
                Nombre = txt.Trim(),
                FamiliaId = (cmbFamilia.SelectedItem as Familia)?.Id,
                UbicacionProductoId = (cmbUbicacion.SelectedItem as UbicacionProducto)?.Id,
                TipoSoporteId = (cmbTipoSoporte.SelectedItem as TipoSoporte)?.Id
            });

            cmbRepuestoCat.ItemsSource = await _catalogoService.GetAllAsync();
            cmbRepuestoCat.SelectedItem = nuevo;
        }
        private async void btnDelRepuestoCat_Click(object sender, RoutedEventArgs e)
        {
            if (cmbRepuestoCat.SelectedItem is RepuestoCatalogo r)
            {
                await _catalogoService.DeleteAsync(r.Id);
                cmbRepuestoCat.ItemsSource = await _catalogoService.GetAllAsync();
            }
        }
        #endregion

        #region Prefijar/Cargar edición (llamados desde CreatePedidoWindow)
        public void PrefijarCabecera(Familia? fam, UbicacionProducto? ubi, Proveedor? prov, TipoSoporte? sop)
        {
            if (fam != null) cmbFamilia.SelectedItem = fam;
            if (ubi != null) cmbUbicacion.SelectedItem = ubi;
            if (prov != null) cmbProveedor.SelectedItem = prov;
            if (sop != null) cmbTipoSoporte.SelectedItem = sop;
        }

        public void CargarParaEdicion(Repuesto rep)
        {
            _buffer.Clear();
            _buffer.Add(rep);
            _editIndex = 0;

            // Prefija cabecera con los valores del repuesto
            PrefijarCabecera(rep.Familia, rep.UbicacionProducto, rep.Proveedor, rep.TipoSoporte);
            // intentar seleccionar en catálogo el mismo nombre
            if (!string.IsNullOrWhiteSpace(rep.Nombre))
            {
                cmbRepuestoCat.SelectedItem = null;
                var lista = cmbRepuestoCat.ItemsSource as System.Collections.IEnumerable;
                if (lista != null)
                {
                    foreach (var it in lista)
                        if (it is RepuestoCatalogo rc && rc.Nombre.Equals(rep.Nombre, StringComparison.OrdinalIgnoreCase))
                        { cmbRepuestoCat.SelectedItem = rc; break; }
                }
            }
        }
        #endregion

        #region ABM de líneas locales
        private void BtnAgregarRepuesto_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarCabecera() is false) return;
            if (cmbRepuestoCat.SelectedItem is not RepuestoCatalogo rc)
            {
                MessageBox.Show("Selecciona un repuesto del catálogo (+ para crear).");
                return;
            }

            var nuevo = new Repuesto
            {
                Nombre = rc.Nombre,
                Descripcion = "",
                Cantidad = 1,
                Precio = 0,
                TipoRepuesto = TipoRepuestoEnum.Original,

                Proveedor = cmbProveedor.SelectedItem as Proveedor ?? throw new InvalidOperationException(),
                ProveedorId = (cmbProveedor.SelectedItem as Proveedor)!.Id,

                Familia = cmbFamilia.SelectedItem as Familia ?? throw new InvalidOperationException(),
                FamiliaId = (cmbFamilia.SelectedItem as Familia)!.Id,

                UbicacionProducto = cmbUbicacion.SelectedItem as UbicacionProducto ?? throw new InvalidOperationException(),
                UbicacionProductoId = (cmbUbicacion.SelectedItem as UbicacionProducto)!.Id,

                TipoSoporte = cmbTipoSoporte.SelectedItem as TipoSoporte ?? throw new InvalidOperationException(),
                TipoSoporteId = (cmbTipoSoporte.SelectedItem as TipoSoporte)!.Id
            };

            _buffer.Add(nuevo);
            _editIndex = null;
        }

        private void BtnEditarRepuesto_Click(object sender, RoutedEventArgs e)
        {
            if (dgTemp.SelectedItem is not Repuesto sel) { MessageBox.Show("Selecciona una línea."); return; }
            _editIndex = _buffer.IndexOf(sel);

            // Vuelve a prefijar cabecera y catálogo
            PrefijarCabecera(sel.Familia, sel.UbicacionProducto, sel.Proveedor, sel.TipoSoporte);
            CargarParaEdicion(sel);
        }

        private void BtnEliminarRepuesto_Click(object sender, RoutedEventArgs e)
        {
            if (dgTemp.SelectedItem is not Repuesto sel) return;
            _buffer.Remove(sel);
            _editIndex = null;
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private bool ValidarCabecera()
        {
            if (cmbFamilia.SelectedItem is not Familia) { MessageBox.Show("Familia requerida."); return false; }
            if (cmbUbicacion.SelectedItem is not UbicacionProducto) { MessageBox.Show("Ubicación requerida."); return false; }
            if (cmbProveedor.SelectedItem is not Proveedor) { MessageBox.Show("Proveedor requerido."); return false; }
            if (cmbTipoSoporte.SelectedItem is not TipoSoporte) { MessageBox.Show("Tipo de soporte requerido."); return false; }
            return true;
        }
        #endregion
    }
}

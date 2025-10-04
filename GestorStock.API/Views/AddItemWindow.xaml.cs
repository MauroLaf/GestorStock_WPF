using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Model.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic; // Para Interaction.InputBox

namespace GestorStock.API.Views
{
    public partial class AddItemWindow : Window
    {
        private readonly StockDbContext _ctx;

        private readonly ObservableCollection<Repuesto> _lineas = new();
        private int _editIndex = -1;

        // Exponer resultados al CreatePedidoWindow
        public Repuesto? RepuestoCreado => _lineas.FirstOrDefault();
        public System.Collections.Generic.IReadOnlyList<Repuesto> RepuestosCreados => _lineas.ToList();

        public AddItemWindow(StockDbContext ctx)
        {
            InitializeComponent();
            _ctx = ctx;

            Loaded += AddItemWindow_Loaded;
            dgRepuestos.ItemsSource = _lineas;
        }

        private async void AddItemWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarCombosAsync();
        }

        #region Carga de combos
        private async Task CargarCombosAsync()
        {
            cmbFamilia.ItemsSource = await _ctx.Familias.AsNoTracking().OrderBy(f => f.Nombre).ToListAsync();
            cmbProveedor.ItemsSource = await _ctx.Proveedores.AsNoTracking().OrderBy(p => p.Nombre).ToListAsync();
            cmbTipoSoporte.ItemsSource = await _ctx.TipoSoportes.AsNoTracking().OrderBy(t => t.Nombre).ToListAsync();
            cmbTipoRepuesto.ItemsSource = Enum.GetValues(typeof(TipoRepuestoEnum));

            if (cmbFamilia.SelectedItem is Familia) await CargarUbicacionesAsync();
        }

        private async Task CargarUbicacionesAsync()
        {
            cmbUbicacion.ItemsSource = null;
            if (cmbFamilia.SelectedItem is Familia fam)
            {
                cmbUbicacion.ItemsSource = await _ctx.UbicacionProductos
                    .Where(u => u.FamiliaId == fam.Id)
                    .OrderBy(u => u.Nombre)
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        private async void cmbFamilia_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
            => await CargarUbicacionesAsync();
        #endregion

        #region + / - PROVEEDOR
        private async void BtnAddProveedor_Click(object sender, RoutedEventArgs e)
        {
            var nombre = (Interaction.InputBox("Nombre del proveedor:", "Añadir Proveedor", "") ?? "").Trim();
            if (string.IsNullOrWhiteSpace(nombre)) return;

            var existe = await _ctx.Proveedores.AnyAsync(p => p.Nombre == nombre);
            if (existe)
            {
                MessageBox.Show("Ya existe un proveedor con ese nombre.", "Duplicado",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var nuevo = new Proveedor { Nombre = nombre };
            _ctx.Proveedores.Add(nuevo);
            await _ctx.SaveChangesAsync();

            cmbProveedor.ItemsSource = await _ctx.Proveedores.AsNoTracking().OrderBy(p => p.Nombre).ToListAsync();
            cmbProveedor.SelectedItem = (cmbProveedor.ItemsSource as System.Collections.Generic.IEnumerable<Proveedor>)?
                .FirstOrDefault(p => p.ProveedorId == nuevo.ProveedorId);
        }

        private async void BtnDelProveedor_Click(object sender, RoutedEventArgs e)
        {
            if (cmbProveedor.SelectedItem is not Proveedor prov)
            {
                MessageBox.Show("Selecciona un proveedor.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var enUso = await _ctx.Repuestos.AnyAsync(r => r.ProveedorId == prov.ProveedorId);
            if (enUso)
            {
                MessageBox.Show("No se puede eliminar: el proveedor está en uso por algún repuesto.",
                    "Operación no permitida", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (MessageBox.Show($"¿Eliminar proveedor '{prov.Nombre}'?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

            _ctx.Proveedores.Remove(prov);
            await _ctx.SaveChangesAsync();

            cmbProveedor.ItemsSource = await _ctx.Proveedores.AsNoTracking().OrderBy(p => p.Nombre).ToListAsync();
            cmbProveedor.SelectedIndex = -1;
        }
        #endregion

        #region + / - FAMILIA
        private async void BtnAddFamilia_Click(object sender, RoutedEventArgs e)
        {
            var nombre = (Interaction.InputBox("Nombre de la familia:", "Añadir Familia", "") ?? "").Trim();
            if (string.IsNullOrWhiteSpace(nombre)) return;

            var existe = await _ctx.Familias.AnyAsync(f => f.Nombre == nombre);
            if (existe)
            {
                MessageBox.Show("Ya existe una familia con ese nombre.", "Duplicado",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var nueva = new Familia { Nombre = nombre };
            _ctx.Familias.Add(nueva);
            await _ctx.SaveChangesAsync();

            cmbFamilia.ItemsSource = await _ctx.Familias.AsNoTracking().OrderBy(f => f.Nombre).ToListAsync();
            cmbFamilia.SelectedItem = (cmbFamilia.ItemsSource as System.Collections.Generic.IEnumerable<Familia>)?
                .FirstOrDefault(f => f.Id == nueva.Id);

            await CargarUbicacionesAsync();
        }

        private async void BtnDelFamilia_Click(object sender, RoutedEventArgs e)
        {
            if (cmbFamilia.SelectedItem is not Familia fam)
            {
                MessageBox.Show("Selecciona una familia.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var enUso = await _ctx.Repuestos.AnyAsync(r => r.FamiliaId == fam.Id)
                        || await _ctx.UbicacionProductos.AnyAsync(u => u.FamiliaId == fam.Id);
            if (enUso)
            {
                MessageBox.Show("No se puede eliminar: hay ubicaciones o repuestos asociados.",
                    "Operación no permitida", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (MessageBox.Show($"¿Eliminar familia '{fam.Nombre}'?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

            _ctx.Familias.Remove(fam);
            await _ctx.SaveChangesAsync();

            cmbFamilia.ItemsSource = await _ctx.Familias.AsNoTracking().OrderBy(f => f.Nombre).ToListAsync();
            cmbFamilia.SelectedIndex = -1;
            cmbUbicacion.ItemsSource = null;
        }
        #endregion

        #region + / - UBICACION
        private async void BtnAddUbicacion_Click(object sender, RoutedEventArgs e)
        {
            if (cmbFamilia.SelectedItem is not Familia fam)
            {
                MessageBox.Show("Selecciona primero una familia.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var nombre = (Interaction.InputBox("Nombre de la nueva ubicación/producto:",
                                               "Añadir Ubicación", "") ?? "").Trim();
            if (string.IsNullOrWhiteSpace(nombre)) return;

            var existe = await _ctx.UbicacionProductos.AnyAsync(u => u.FamiliaId == fam.Id && u.Nombre == nombre);
            if (existe)
            {
                MessageBox.Show("Ya existe una ubicación con ese nombre en esa familia.",
                    "Duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var nueva = new UbicacionProducto { Nombre = nombre, FamiliaId = fam.Id };
            _ctx.UbicacionProductos.Add(nueva);
            await _ctx.SaveChangesAsync();

            await CargarUbicacionesAsync();
            cmbUbicacion.SelectedItem = (cmbUbicacion.ItemsSource as System.Collections.Generic.IEnumerable<UbicacionProducto>)?
                .FirstOrDefault(u => u.Id == nueva.Id);
        }

        private async void BtnDelUbicacion_Click(object sender, RoutedEventArgs e)
        {
            if (cmbUbicacion.SelectedItem is not UbicacionProducto ubi)
            {
                MessageBox.Show("Selecciona una ubicación.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var enUso = await _ctx.Repuestos.AnyAsync(r => r.UbicacionProductoId == ubi.Id);
            if (enUso)
            {
                MessageBox.Show("No se puede eliminar: la ubicación está en uso por algún repuesto.",
                    "Operación no permitida", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (MessageBox.Show($"¿Eliminar ubicación '{ubi.Nombre}'?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

            _ctx.UbicacionProductos.Remove(ubi);
            await _ctx.SaveChangesAsync();
            await CargarUbicacionesAsync();
        }
        #endregion

        #region + / - TIPO SOPORTE
        private async void BtnAddTipoSoporte_Click(object sender, RoutedEventArgs e)
        {
            var nombre = (Interaction.InputBox("Nombre del tipo de soporte:",
                                               "Añadir Tipo de Soporte", "") ?? "").Trim();
            if (string.IsNullOrWhiteSpace(nombre)) return;

            var existe = await _ctx.TipoSoportes.AnyAsync(t => t.Nombre == nombre);
            if (existe)
            {
                MessageBox.Show("Ya existe un tipo de soporte con ese nombre.", "Duplicado",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var nuevo = new TipoSoporte { Nombre = nombre };
            _ctx.TipoSoportes.Add(nuevo);
            await _ctx.SaveChangesAsync();

            cmbTipoSoporte.ItemsSource = await _ctx.TipoSoportes.AsNoTracking().OrderBy(t => t.Nombre).ToListAsync();
            cmbTipoSoporte.SelectedItem = (cmbTipoSoporte.ItemsSource as System.Collections.Generic.IEnumerable<TipoSoporte>)?
                .FirstOrDefault(t => t.Id == nuevo.Id);
        }

        private async void BtnDelTipoSoporte_Click(object sender, RoutedEventArgs e)
        {
            if (cmbTipoSoporte.SelectedItem is not TipoSoporte sop)
            {
                MessageBox.Show("Selecciona un tipo de soporte.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var enUso = await _ctx.Repuestos.AnyAsync(r => r.TipoSoporteId == sop.Id);
            if (enUso)
            {
                MessageBox.Show("No se puede eliminar: hay repuestos asociados a este tipo de soporte.",
                    "Operación no permitida", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (MessageBox.Show($"¿Eliminar tipo de soporte '{sop.Nombre}'?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

            _ctx.TipoSoportes.Remove(sop);
            await _ctx.SaveChangesAsync();

            cmbTipoSoporte.ItemsSource = await _ctx.TipoSoportes.AsNoTracking().OrderBy(t => t.Nombre).ToListAsync();
            cmbTipoSoporte.SelectedIndex = -1;
        }
        #endregion

        #region + / - TIPO REPUESTO (ENUM)
        private void BtnAddTipoRepuesto_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("El 'Tipo de Repuesto' es un enumerado fijo (Original/Compatible). " +
                            "No se pueden añadir valores en tiempo de ejecución.",
                            "Información", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDelTipoRepuesto_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("El 'Tipo de Repuesto' es un enumerado fijo (Original/Compatible). " +
                            "No se pueden eliminar valores en tiempo de ejecución.",
                            "Información", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        #region Agregar / Editar / Actualizar / Eliminar línea
        private void BtnAgregarRepuesto_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarCabecera(out var fam, out var ubi, out var prov, out var sop, out var tipo))
                return;

            var (cantidad, precio) = ParseCantidadPrecio();

            var nuevo = new Repuesto
            {
                Nombre = txtNombre.Text?.Trim() ?? "",
                Descripcion = "",
                Cantidad = cantidad,
                Precio = precio,
                TipoRepuesto = tipo,
                FamiliaId = fam.Id,
                UbicacionProductoId = ubi.Id,
                ProveedorId = prov.ProveedorId,
                TipoSoporteId = sop.Id
            };

            _lineas.Add(nuevo);
            LimpiarFormulario();
        }

        private void BtnEditarRepuesto_Click(object sender, RoutedEventArgs e)
        {
            if (dgRepuestos.SelectedItem is not Repuesto sel) return;
            _editIndex = dgRepuestos.SelectedIndex;

            // Form
            txtNombre.Text = sel.Nombre;
            txtPrecio.Text = sel.Precio.ToString();
            txtCantidad.Text = sel.Cantidad.ToString();
            cmbTipoRepuesto.SelectedItem = sel.TipoRepuesto;

            cmbFamilia.SelectedItem = (cmbFamilia.ItemsSource as System.Collections.Generic.IEnumerable<Familia>)?
                .FirstOrDefault(f => f.Id == sel.FamiliaId);

            _ = CargarUbicacionesAsync().ContinueWith(_ =>
            {
                Dispatcher.Invoke(() =>
                {
                    cmbUbicacion.SelectedItem = (cmbUbicacion.ItemsSource as System.Collections.Generic.IEnumerable<UbicacionProducto>)?
                        .FirstOrDefault(u => u.Id == sel.UbicacionProductoId);
                });
            });

            cmbProveedor.SelectedItem = (cmbProveedor.ItemsSource as System.Collections.Generic.IEnumerable<Proveedor>)?
                .FirstOrDefault(p => p.ProveedorId == sel.ProveedorId);

            cmbTipoSoporte.SelectedItem = (cmbTipoSoporte.ItemsSource as System.Collections.Generic.IEnumerable<TipoSoporte>)?
                .FirstOrDefault(t => t.Id == sel.TipoSoporteId);

            BtnActualizarRepuesto.Visibility = Visibility.Visible;
            BtnAgregarRepuesto.IsEnabled = false;
        }

        private void BtnActualizarRepuesto_Click(object sender, RoutedEventArgs e)
        {
            if (_editIndex < 0 || _editIndex >= _lineas.Count) return;

            if (!ValidarCabecera(out var fam, out var ubi, out var prov, out var sop, out var tipo))
                return;

            var (cantidad, precio) = ParseCantidadPrecio();

            var destino = _lineas[_editIndex];
            destino.Nombre = txtNombre.Text?.Trim() ?? "";
            destino.Cantidad = cantidad;
            destino.Precio = precio;
            destino.TipoRepuesto = tipo;
            destino.FamiliaId = fam.Id;
            destino.UbicacionProductoId = ubi.Id;
            destino.ProveedorId = prov.ProveedorId;
            destino.TipoSoporteId = sop.Id;

            dgRepuestos.Items.Refresh();
            ResetEdicion();
        }

        private void BtnEliminarRepuesto_Click(object sender, RoutedEventArgs e)
        {
            if (dgRepuestos.SelectedItem is not Repuesto sel) return;
            _lineas.Remove(sel);
            dgRepuestos.Items.Refresh();
            ResetEdicion();
        }
        #endregion

        #region Aceptar / Cancelar
        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            // El padre (CreatePedidoWindow) recogerá RepuestosCreados
            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        #endregion

        #region Helpers
        private bool ValidarCabecera(out Familia fam, out UbicacionProducto ubi,
                                     out Proveedor prov, out TipoSoporte sop,
                                     out TipoRepuestoEnum tipo)
        {
            fam = default!; ubi = default!; prov = default!; sop = default!; tipo = default;

            if (cmbFamilia.SelectedItem is not Familia f ||
                cmbUbicacion.SelectedItem is not UbicacionProducto u ||
                cmbProveedor.SelectedItem is not Proveedor p ||
                cmbTipoSoporte.SelectedItem is not TipoSoporte s ||
                cmbTipoRepuesto.SelectedItem is not TipoRepuestoEnum t)
            {
                MessageBox.Show("Completa Proveedor, Familia, Ubicación, Tipo de Soporte y Tipo de Repuesto.",
                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            fam = f; ubi = u; prov = p; sop = s; tipo = t;
            return true;
        }

        private (int cantidad, decimal precio) ParseCantidadPrecio()
        {
            if (!int.TryParse(txtCantidad.Text, out var cantidad) || cantidad <= 0) cantidad = 1;
            if (!decimal.TryParse(txtPrecio.Text, out var precio) || precio < 0) precio = 0m;
            return (cantidad, precio);
        }

        private void LimpiarFormulario()
        {
            txtNombre.Clear();
            txtPrecio.Clear();
            txtCantidad.Clear();
            cmbTipoRepuesto.SelectedIndex = -1;
        }

        private void ResetEdicion()
        {
            _editIndex = -1;
            BtnActualizarRepuesto.Visibility = Visibility.Collapsed;
            BtnAgregarRepuesto.IsEnabled = true;
            LimpiarFormulario();
        }
        #endregion
    }
}

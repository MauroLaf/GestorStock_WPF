using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GestorStock.Model.Entities;
using GestorStock.Model.Enum;

namespace GestorStock.API.Views
{
    public partial class AddItemWindow : Window
    {
        public ObservableCollection<Repuesto> RepuestosCreados { get; } = new();

        public AddItemWindow()
        {
            InitializeComponent();
            dgDetalle.ItemsSource = RepuestosCreados;
            cmbTipoRepuesto.SelectedIndex = 0; // Original
        }

        // Prefija combos de cabecera desde la ventana padre
        public void PrefijarCabecera(Familia? fam, UbicacionProducto? ubi, Proveedor? prov, TipoSoporte? sop)
        {
            cmbFamilia.ItemsSource = fam != null ? new[] { fam } : null;
            cmbUbicacion.ItemsSource = ubi != null ? new[] { ubi } : null;
            cmbProveedor.ItemsSource = prov != null ? new[] { prov } : null;
            cmbTipoSoporte.ItemsSource = sop != null ? new[] { sop } : null;

            if (fam != null) cmbFamilia.SelectedItem = fam;
            if (ubi != null) cmbUbicacion.SelectedItem = ubi;
            if (prov != null) cmbProveedor.SelectedItem = prov;
            if (sop != null) cmbTipoSoporte.SelectedItem = sop;
        }

        private Repuesto? _edicion; // línea que se está editando

        public void CargarParaEdicion(Repuesto r)
        {
            _edicion = r;
            cmbRepuestoCatalogo.Text = r.Nombre;
            txtDescripcion.Text = r.Descripcion;
            txtCantidad.Text = r.Cantidad.ToString();
            txtPrecio.Text = r.Precio.ToString("0.##");
            cmbTipoRepuesto.SelectedIndex = r.TipoRepuesto == TipoRepuestoEnum.Original ? 0 : 1;

            // Cabecera (si vienen nulos, usa los de la línea)
            if (cmbFamilia.SelectedItem == null && r.Familia != null) cmbFamilia.ItemsSource = new[] { r.Familia };
            if (cmbUbicacion.SelectedItem == null && r.UbicacionProducto != null) cmbUbicacion.ItemsSource = new[] { r.UbicacionProducto };
            if (cmbProveedor.SelectedItem == null && r.Proveedor != null) cmbProveedor.ItemsSource = new[] { r.Proveedor };
            if (cmbTipoSoporte.SelectedItem == null && r.TipoSoporte != null) cmbTipoSoporte.ItemsSource = new[] { r.TipoSoporte };

            if (r.Familia != null) cmbFamilia.SelectedItem = r.Familia;
            if (r.UbicacionProducto != null) cmbUbicacion.SelectedItem = r.UbicacionProducto;
            if (r.Proveedor != null) cmbProveedor.SelectedItem = r.Proveedor;
            if (r.TipoSoporte != null) cmbTipoSoporte.SelectedItem = r.TipoSoporte;
        }

        private void BtnAddLinea_Click(object sender, RoutedEventArgs e)
        {
            if (!TryBuildRepuesto(out var rep)) return;
            RepuestosCreados.Add(rep);
            LimpiarLinea();
        }

        private void BtnEditLinea_Click(object sender, RoutedEventArgs e)
        {
            if (dgDetalle.SelectedItem is not Repuesto sel)
            {
                // si venimos desde editar de la ventana padre _edicion no es null
                if (_edicion == null) { MessageBox.Show("Selecciona una línea."); return; }
                sel = _edicion;
            }

            if (!TryBuildRepuesto(out var editado)) return;

            var idx = RepuestosCreados.IndexOf(sel);
            if (idx >= 0) RepuestosCreados[idx] = editado;

            _edicion = null;
            LimpiarLinea();
        }

        private void BtnRemoveLinea_Click(object sender, RoutedEventArgs e)
        {
            if (dgDetalle.SelectedItem is Repuesto sel)
                RepuestosCreados.Remove(sel);
        }

        private bool TryBuildRepuesto(out Repuesto rep)
        {
            rep = new Repuesto();

            if (cmbFamilia.SelectedItem is not Familia fam
             || cmbUbicacion.SelectedItem is not UbicacionProducto ubi
             || cmbProveedor.SelectedItem is not Proveedor prov
             || cmbTipoSoporte.SelectedItem is not TipoSoporte sop)
            {
                MessageBox.Show("Completa la cabecera (Familia, Ubicación, Proveedor, Soporte).");
                return false;
            }

            if (!int.TryParse(txtCantidad.Text.Trim(), out var cant) || cant <= 0)
            {
                MessageBox.Show("Cantidad inválida."); return false;
            }

            if (!decimal.TryParse(txtPrecio.Text.Trim(), out var precio) || precio < 0)
            {
                MessageBox.Show("Precio inválido."); return false;
            }

            rep.Nombre = (cmbRepuestoCatalogo.Text ?? "").Trim();
            rep.Descripcion = (txtDescripcion.Text ?? "").Trim();
            rep.Cantidad = cant;
            rep.Precio = precio;
            rep.TipoRepuesto = (cmbTipoRepuesto.SelectedIndex == 0) ? TipoRepuestoEnum.Original : TipoRepuestoEnum.Usado;

            rep.FamiliaId = fam.Id; rep.Familia = fam;
            rep.UbicacionProductoId = ubi.Id; rep.UbicacionProducto = ubi;
            rep.ProveedorId = prov.Id; rep.Proveedor = prov;
            rep.TipoSoporteId = sop.Id; rep.TipoSoporte = sop;

            return true;
        }

        private void LimpiarLinea()
        {
            cmbRepuestoCatalogo.Text = "";
            txtDescripcion.Text = "";
            txtCantidad.Text = "1";
            txtPrecio.Text = "0";
            cmbTipoRepuesto.SelectedIndex = 0;
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => Close();
    }
}

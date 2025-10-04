using System;
using System.Linq;
using System.Windows;
using GestorStock.Data;
using GestorStock.Model.Entities;
using GestorStock.Model.Enum;
using Microsoft.EntityFrameworkCore;

namespace GestorStock.API.Views
{
    public partial class AddItemWindow : Window
    {
        private readonly StockDbContext _ctx;

        public AddItemWindow(StockDbContext ctx)
        {
            InitializeComponent();
            _ctx = ctx;
            Loaded += AddItemWindow_Loaded;
        }

        private async void AddItemWindow_Loaded(object sender, RoutedEventArgs e)
        {
            cmbFamilia.ItemsSource = await _ctx.Familias.AsNoTracking().OrderBy(f => f.Nombre).ToListAsync();
            cmbProveedor.ItemsSource = await _ctx.Proveedores.AsNoTracking().OrderBy(p => p.Nombre).ToListAsync();
            cmbTipoSoporte.ItemsSource = await _ctx.TipoSoportes.AsNoTracking().OrderBy(t => t.Nombre).ToListAsync();
            cmbTipoRepuesto.ItemsSource = Enum.GetValues(typeof(TipoRepuestoEnum));
        }

        private async void cmbFamilia_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cmbFamilia.SelectedItem is Familia fam)
            {
                cmbUbicacion.ItemsSource = await _ctx.UbicacionProductos
                    .Where(u => u.FamiliaId == fam.Id)
                    .OrderBy(u => u.Nombre)
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        // Devuelve el repuesto creado a la ventana que la llamó (por ejemplo, CreatePedidoWindow)
        public Repuesto? RepuestoCreado { get; private set; }

        private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbFamilia.SelectedItem is not Familia fam ||
                cmbUbicacion.SelectedItem is not UbicacionProducto ubi ||
                cmbProveedor.SelectedItem is not Proveedor prov ||
                cmbTipoSoporte.SelectedItem is not TipoSoporte sop ||
                cmbTipoRepuesto.SelectedItem is not TipoRepuestoEnum tipo)
            {
                MessageBox.Show("Completa todos los campos.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0) cantidad = 1;
            if (!decimal.TryParse(txtPrecio.Text, out decimal precio) || precio < 0) precio = 0;

            RepuestoCreado = new Repuesto
            {
                Nombre = txtNombre.Text?.Trim() ?? "",
                Descripcion = txtDescripcion.Text?.Trim() ?? "",
                Cantidad = cantidad,
                Precio = precio,
                TipoRepuesto = tipo,
                FamiliaId = fam.Id,
                UbicacionProductoId = ubi.Id,
                ProveedorId = prov.ProveedorId,
                TipoSoporteId = sop.Id
            };

            // Si quieres persistir aquí, descomenta:
            // _ctx.Repuestos.Add(RepuestoCreado);
            // await _ctx.SaveChangesAsync();

            DialogResult = true;
            Close();
        }
    }
}

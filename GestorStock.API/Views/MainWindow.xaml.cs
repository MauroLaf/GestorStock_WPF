using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using OfficeOpenXml;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GestorStock.API.Views
{
    [SupportedOSPlatform("windows")]
    public partial class MainWindow : Window
    {
        private readonly IFamiliaService _familiaService;
        private readonly ITipoSoporteService _tipoSoporteService;
        private readonly IPedidoService _pedidoService;
        private readonly IRepuestoService _repuestoService;
        private readonly IProveedorService _proveedorService;
        private readonly IUbicacionProductoService _ubicacionProductoService;
        private readonly IServiceProvider _sp;

        private readonly ObservableCollection<Pedido> _pedidos = new();
        private bool _isExporting;

        public MainWindow(
            IPedidoService pedidoService,
            IRepuestoService repuestoService,
            IFamiliaService familiaService,
            ITipoSoporteService tipoSoporteService,
            IProveedorService proveedorService,
            IUbicacionProductoService ubicacionProductoService,
            IServiceProvider sp)
        {
            InitializeComponent();

            _pedidoService = pedidoService;
            _repuestoService = repuestoService;
            _familiaService = familiaService;
            _tipoSoporteService = tipoSoporteService;
            _proveedorService = proveedorService;
            _ubicacionProductoService = ubicacionProductoService;
            _sp = sp;

            PedidosDataGrid.ItemsSource = _pedidos;

            Loaded += MainWindow_Loaded;
            CreateButton.Click += CreateButton_Click;
            EditButton.Click += EditButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            BuscarButton.Click += BuscarButton_Click;
            LimpiarButton.Click += LimpiarButton_Click;
            ExportarExcelButton.Click += ExportarExcelButton_Click;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarFamiliasAsync();
            await CargarTiposSoporteAsync();
            await CargarTodosLosPedidosAsync();
        }

        private async Task CargarFamiliasAsync()
        {
            try
            {
                var familias = await _familiaService.GetAllAsync();
                FamiliaComboBox.ItemsSource = familias;
                FamiliaComboBox.DisplayMemberPath = "Nombre";
                FamiliaComboBox.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar Familias: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task CargarTiposSoporteAsync()
        {
            try
            {
                var tipos = await _tipoSoporteService.GetAllAsync();
                TipoSoporteComboBox.ItemsSource = tipos;
                TipoSoporteComboBox.DisplayMemberPath = "Nombre";
                TipoSoporteComboBox.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar Tipos de Soporte: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task CargarTodosLosPedidosAsync()
        {
            try
            {
                var pedidos = await _pedidoService.GetAllWithDetalleAsync();
                _pedidos.Clear();
                foreach (var p in pedidos) _pedidos.Add(p);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar pedidos: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BuscarButton_Click(object sender, RoutedEventArgs e)
        {
            await CargarTodosLosPedidosAsync();

            var fam = FamiliaComboBox.SelectedItem as Familia;
            var sop = TipoSoporteComboBox.SelectedItem as TipoSoporte;

            var filtrados = _pedidos.Where(p =>
                (fam == null || p.Repuestos.Any(r => r.UbicacionProducto?.FamiliaId == fam.Id)) &&
                (sop == null || p.Repuestos.Any(r => r.TipoSoporteId == sop.Id))
            ).ToList();

            _pedidos.Clear();
            foreach (var p in filtrados) _pedidos.Add(p);
        }

        private async void LimpiarButton_Click(object sender, RoutedEventArgs e)
        {
            FamiliaComboBox.SelectedIndex = -1;
            TipoSoporteComboBox.SelectedIndex = -1;
            await CargarTodosLosPedidosAsync();
        }

        // === CREAR ===
        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var win = new CreatePedidoWindow(
                _pedidoService,
                _repuestoService,
                _familiaService,
                _tipoSoporteService,
                _proveedorService,
                _ubicacionProductoService,
                _sp,
                null);

            win.Owner = this;

            if (win.ShowDialog() == true)
                await CargarTodosLosPedidosAsync();
        }

        // === EDITAR ===
        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (PedidosDataGrid.SelectedItem is not Pedido seleccionado)
            {
                MessageBox.Show("Selecciona un pedido.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var pedidoCompleto = await _pedidoService.GetWithDetalleAsync(seleccionado.Id);
            if (pedidoCompleto is null)
            {
                MessageBox.Show("No se pudo cargar el pedido.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var win = new CreatePedidoWindow(
                _pedidoService,
                _repuestoService,
                _familiaService,
                _tipoSoporteService,
                _proveedorService,
                _ubicacionProductoService,
                _sp,
                pedidoCompleto);

            win.Owner = this;

            if (win.ShowDialog() == true)
                await CargarTodosLosPedidosAsync();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (PedidosDataGrid.SelectedItem is not Pedido seleccionado) return;

            if (MessageBox.Show("¿Eliminar el pedido seleccionado?",
                "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                await _pedidoService.DeleteAsync(seleccionado.Id);
                await CargarTodosLosPedidosAsync();
            }
        }

        private async void EliminarRepuesto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int repuestoId)
            {
                if (MessageBox.Show("¿Eliminar este repuesto?", "Confirmar",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await _repuestoService.DeleteAsync(repuestoId);
                    await CargarTodosLosPedidosAsync();
                }
            }
        }

        // === Exportar Excel con EPPlus 8 ===
        private void ExportarExcelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isExporting) return;
            _isExporting = true;
            ExportarExcelButton.IsEnabled = false;

            try
            {
                var dlg = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Archivo de Excel (*.xlsx)|*.xlsx",
                    FileName = "Pedidos.xlsx",
                    OverwritePrompt = true
                };

                if (dlg.ShowDialog() != true) return;

                var file = new FileInfo(dlg.FileName);
                using var package = new ExcelPackage(file);

                var name = "Pedidos";
                var ws = package.Workbook.Worksheets.FirstOrDefault(w => w.Name == name)
                         ?? package.Workbook.Worksheets.Add(name);
                ws.Cells.Clear();

                string[] headers = {
                    "ID Pedido","Fecha Creación","Descripción","Incidencia","Fecha Incidencia","Fecha Llegada",
                    "Ubicación","Familia","Tipo Soporte",
                    "ID Repuesto","Nombre","Cantidad","Precio","Total Línea"
                };

                for (int c = 0; c < headers.Length; c++) ws.Cells[1, c + 1].Value = headers[c];

                int row = 2;
                foreach (var p in _pedidos)
                {
                    if (p.Repuestos == null || !p.Repuestos.Any())
                    {
                        ws.Cells[row, 1].Value = p.Id;
                        ws.Cells[row, 2].Value = p.FechaCreacion;
                        ws.Cells[row, 3].Value = p.Descripcion;
                        ws.Cells[row, 4].Value = p.Incidencia;
                        ws.Cells[row, 6].Value = p.FechaLlegada;
                        row++;
                        continue;
                    }

                    foreach (var r in p.Repuestos)
                    {
                        ws.Cells[row, 1].Value = p.Id;
                        ws.Cells[row, 2].Value = p.FechaCreacion;
                        ws.Cells[row, 3].Value = p.Descripcion;
                        ws.Cells[row, 4].Value = p.Incidencia;
                        ws.Cells[row, 5].Value = p.FechaIncidencia;
                        ws.Cells[row, 6].Value = p.FechaLlegada;
                        ws.Cells[row, 7].Value = r.UbicacionProducto?.Nombre;
                        ws.Cells[row, 8].Value = r.UbicacionProducto?.Familia?.Nombre;
                        ws.Cells[row, 9].Value = r.TipoSoporte?.Nombre;

                        ws.Cells[row, 10].Value = r.Id;
                        ws.Cells[row, 11].Value = r.Nombre;
                        ws.Cells[row, 12].Value = r.Cantidad;
                        ws.Cells[row, 13].Value = r.Precio;
                        ws.Cells[row, 14].Value = r.Cantidad * r.Precio;
                        row++;
                    }
                }

                if (ws.Dimension != null) ws.Cells[ws.Dimension.Address].AutoFitColumns();
                package.Save();
                MessageBox.Show("Exportado correctamente.", "Éxito",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isExporting = false;
                ExportarExcelButton.IsEnabled = true;
            }
        }
    }
}

using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection; // GetRequiredService

namespace GestorStock.API.Views
{
    [SupportedOSPlatform("windows")]
    public partial class MainWindow : Window
    {
        private readonly IFamiliaService _familiaService;
        private readonly ITipoSoporteService _tipoSoporteService;
        private readonly IPedidoService _pedidoService;
        private readonly IRepuestoService _repuestoService;
        private readonly IServiceProvider _sp;   // Resolver ventanas por DI (opción A)

        private readonly ObservableCollection<Pedido> _pedidos = new();
        private bool _isExporting = false;

        public MainWindow(
            IPedidoService pedidoService,
            IRepuestoService repuestoService,
            IFamiliaService familiaService,
            ITipoSoporteService tipoSoporteService,
            IServiceProvider sp)
        {
            InitializeComponent();

            _pedidoService = pedidoService;
            _repuestoService = repuestoService;
            _familiaService = familiaService;
            _tipoSoporteService = tipoSoporteService;
            _sp = sp;

            PedidosDataGrid.ItemsSource = _pedidos;

            Loaded += MainWindow_Loaded;

            CreateButton.Click += CreateButton_Click;
            EditButton.Click += EditButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            BuscarButton.Click += BuscarButton_Click;
            LimpiarButton.Click += LimpiarButton_Click;
            PedidosDataGrid.SelectionChanged += PedidosDataGrid_SelectionChanged;
            ExportarExcelButton.Click += ExportarExcelButton_Click;
        }

        private async void MainWindow_Loaded(object? sender, RoutedEventArgs e)
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
                MessageBox.Show($"Error al cargar las Familias: {ex.Message}", "Error",
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
                MessageBox.Show($"Error al cargar los tipos de soporte: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // === ÚNICA definición de este método (evita duplicados) ===
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
                MessageBox.Show($"Error al cargar los pedidos: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BuscarButton_Click(object sender, RoutedEventArgs e)
        {
            await CargarTodosLosPedidosAsync();

            var famSel = FamiliaComboBox.SelectedItem as Model.Entities.Familia;
            var sopSel = TipoSoporteComboBox.SelectedItem as Model.Entities.TipoSoporte;

            var resultados = _pedidos.Where(p =>
                (famSel == null || p.Repuestos.Any(r => r.UbicacionProducto?.FamiliaId == famSel.Id)) &&
                (sopSel == null || p.Repuestos.Any(r => r.TipoSoporteId == sopSel.Id))
            ).ToList();

            _pedidos.Clear();
            foreach (var p in resultados) _pedidos.Add(p);
        }

        private async void LimpiarButton_Click(object sender, RoutedEventArgs e)
        {
            FamiliaComboBox.SelectedIndex = -1;
            TipoSoporteComboBox.SelectedIndex = -1;
            await CargarTodosLosPedidosAsync();
        }

        // === Crear ===
        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var win = _sp.GetRequiredService<CreatePedidoWindow>();
            win.Owner = this;

            if (win.ShowDialog() == true)
                await CargarTodosLosPedidosAsync();
        }

        // === Editar ===
        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (PedidosDataGrid.SelectedItem is not Pedido sel)
            {
                MessageBox.Show("Selecciona un pedido.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var pedidoCompleto = await _pedidoService.GetWithDetalleAsync(sel.Id);
            if (pedidoCompleto == null)
            {
                MessageBox.Show("No se pudo cargar el pedido.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var win = _sp.GetRequiredService<CreatePedidoWindow>();
            win.Owner = this;
            win.PedidoEditar = pedidoCompleto;   // <- propiedad en CreatePedidoWindow (ver punto 2)

            if (win.ShowDialog() == true)
                await CargarTodosLosPedidosAsync();
        }

        // === Eliminar Pedido ===
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (PedidosDataGrid.SelectedItem is not Pedido sel) return;

            if (MessageBox.Show("¿Eliminar este pedido?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                await _pedidoService.DeleteAsync(sel.Id);
                await CargarTodosLosPedidosAsync();
            }
        }

        // === Eliminar Repuesto (fila detalle) ===
        private async void EliminarRepuesto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int repuestoId)
            {
                if (MessageBox.Show("¿Eliminar este repuesto?", "Confirmar",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

                try
                {
                    await _repuestoService.DeleteAsync(repuestoId);
                    await CargarTodosLosPedidosAsync();
                    MessageBox.Show("Repuesto eliminado.", "OK",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error eliminando repuesto");
                }
            }
        }

        private void PedidosDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        // === Exportar a Excel (EPPlus 8) ===
        private void ExportarExcelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isExporting) return;

            _isExporting = true;
            ExportarExcelButton.IsEnabled = false;

            try
            {
                var dlg = new SaveFileDialog
                {
                    Title = "Exportar pedidos",
                    Filter = "Excel (*.xlsx)|*.xlsx",
                    FileName = "Pedidos.xlsx",
                    AddExtension = true,
                    DefaultExt = ".xlsx",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (dlg.ShowDialog() != true) return;

                // EPPlus 8+: configure licencia ANTES de instanciar ExcelPackage
                // Elige una de estas dos:
                // ExcelPackage.License.SetNonCommercialPersonal(Environment.UserName);
                ExcelPackage.License.SetNonCommercialOrganization("GestorStock");

                using var package = new ExcelPackage(new FileInfo(dlg.FileName));
                var sheetName = "Pedidos_Unificado";
                var ws = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == sheetName)
                         ?? package.Workbook.Worksheets.Add(sheetName);

                ws.Cells.Clear();

                string[] headers = {
                    "ID Pedido","Fecha Creación","Descripción Pedido","Incidencia","Fecha Incidencia",
                    "Fecha Llegada","Descripción Incidencia","Ubicación","Tipo Soporte","Familia",
                    "ID Repuesto","Nombre Repuesto","Cantidad","Descripción Repuesto","Precio","Tipo Repuesto"
                };
                for (int i = 0; i < headers.Length; i++) ws.Cells[1, i + 1].Value = headers[i];

                int row = 2;
                foreach (var p in _pedidos)
                {
                    if (p.Repuestos == null || !p.Repuestos.Any())
                    {
                        ws.Cells[row, 1].Value = p.Id;
                        ws.Cells[row, 2].Value = p.FechaCreacion; ws.Cells[row, 2].Style.Numberformat.Format = "dd/MM/yyyy";
                        ws.Cells[row, 3].Value = p.Descripcion;
                        ws.Cells[row, 4].Value = p.Incidencia;
                        ws.Cells[row, 5].Value = p.FechaIncidencia; ws.Cells[row, 5].Style.Numberformat.Format = "dd/MM/yyyy";
                        ws.Cells[row, 6].Value = p.FechaLlegada; ws.Cells[row, 6].Style.Numberformat.Format = "dd/MM/yyyy";
                        ws.Cells[row, 7].Value = p.DescripcionIncidencia;
                        row++;
                    }
                    else
                    {
                        foreach (var r in p.Repuestos)
                        {
                            ws.Cells[row, 1].Value = p.Id;
                            ws.Cells[row, 2].Value = p.FechaCreacion; ws.Cells[row, 2].Style.Numberformat.Format = "dd/MM/yyyy";
                            ws.Cells[row, 3].Value = p.Descripcion;
                            ws.Cells[row, 4].Value = p.Incidencia;
                            ws.Cells[row, 5].Value = p.FechaIncidencia; ws.Cells[row, 5].Style.Numberformat.Format = "dd/MM/yyyy";
                            ws.Cells[row, 6].Value = p.FechaLlegada; ws.Cells[row, 6].Style.Numberformat.Format = "dd/MM/yyyy";
                            ws.Cells[row, 7].Value = p.DescripcionIncidencia;

                            ws.Cells[row, 8].Value = r.UbicacionProducto?.Nombre;
                            ws.Cells[row, 9].Value = r.TipoSoporte?.Nombre;
                            ws.Cells[row, 10].Value = r.UbicacionProducto?.Familia?.Nombre;

                            ws.Cells[row, 11].Value = r.Id;
                            ws.Cells[row, 12].Value = r.Nombre;
                            ws.Cells[row, 13].Value = r.Cantidad;
                            ws.Cells[row, 14].Value = r.Descripcion;
                            ws.Cells[row, 15].Value = r.Precio;
                            ws.Cells[row, 16].Value = r.TipoRepuesto.ToString();
                            row++;
                        }
                    }
                }

                if (ws.Dimension != null) ws.Cells[ws.Dimension.Address].AutoFitColumns();
                package.Save();

                MessageBox.Show("Datos exportados correctamente.", "Éxito",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error al exportar");
            }
            finally
            {
                _isExporting = false;
                ExportarExcelButton.IsEnabled = true;
            }
        }
    }
}

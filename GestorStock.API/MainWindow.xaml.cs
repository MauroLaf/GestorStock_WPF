using GestorStock.Data.Repositories;
using GestorStock.Model.Entities;
using GestorStock.Services.Implementations;
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

namespace GestorStock.API
{
    [SupportedOSPlatform("windows")]
    public partial class MainWindow : Window
    {
        private readonly ITipoItemService _tipoItemService;
        private readonly IPedidoService _pedidoService;
        private readonly IRepuestoService _repuestoService;
        private readonly ITipoExplotacionService _tipoExplotacionService;
        private readonly ITipoRepuestoService _tipoRepuestoService;
        private readonly IItemService _itemService;

        private ObservableCollection<Pedido> _pedidos;
        private bool _isExporting = false; // Variable de control

        public MainWindow(IPedidoService pedidoService, IRepuestoService repuestoService,
            ITipoExplotacionService tipoExplotacionService, ITipoRepuestoService tipoRepuestoService,
            ITipoItemService tipoItemService, IItemService itemService)
        {
            InitializeComponent();

            _pedidoService = pedidoService;
            _repuestoService = repuestoService;
            _tipoExplotacionService = tipoExplotacionService;
            _tipoRepuestoService = tipoRepuestoService;
            _tipoItemService = tipoItemService;
            _itemService = itemService;

            _pedidos = new ObservableCollection<Pedido>();
            PedidosDataGrid.ItemsSource = _pedidos;

            this.Loaded += MainWindow_Loaded;

            CreateButton.Click += CreateButton_Click;
            EditButton.Click += EditButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            BuscarButton.Click += BuscarButton_Click;
            LimpiarButton.Click += LimpiarButton_Click;
            // NOTA: No se suscribe ExportarExcelButton aquí porque ya está en XAML
            PedidosDataGrid.SelectionChanged += PedidosDataGrid_SelectionChanged;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarExplotacionesAsync();
            await CargarTiposItemAsync();
            await CargarTodosLosPedidosAsync();
        }

        private async Task CargarExplotacionesAsync()
        {
            try
            {
                var explotaciones = await _tipoExplotacionService.GetAllTipoExplotacionAsync();
                ExplotacionComboBox.ItemsSource = explotaciones;
                ExplotacionComboBox.DisplayMemberPath = "Nombre";
                ExplotacionComboBox.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar las explotaciones: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task CargarTiposItemAsync()
        {
            try
            {
                var tiposItem = await _tipoItemService.GetAllTipoItemAsync();
                TipoSoporteComboBox.ItemsSource = tiposItem;
                TipoSoporteComboBox.DisplayMemberPath = "Nombre";
                TipoSoporteComboBox.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los tipos de ítem: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task CargarTodosLosPedidosAsync()
        {
            try
            {
                var pedidos = await _pedidoService.GetAllPedidosWithDetailsAsync();
                _pedidos.Clear();
                foreach (var pedido in pedidos)
                {
                    _pedidos.Add(pedido);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los pedidos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BuscarButton_Click(object sender, RoutedEventArgs e)
        {
            await CargarTodosLosPedidosAsync();

            var explotacionSeleccionada = ExplotacionComboBox.SelectedItem as TipoExplotacion;
            var tipoItemSeleccionado = TipoSoporteComboBox.SelectedItem as TipoSoporte;

            var resultados = _pedidos.Where(p =>
                (explotacionSeleccionada == null || p.Items.Any(item => item.TipoExplotacion?.Id == explotacionSeleccionada.Id)) &&
                (tipoItemSeleccionado == null || p.Items.Any(item => item.TipoSoporte?.Id == tipoItemSeleccionado.Id))
            ).ToList();

            _pedidos.Clear();
            foreach (var pedido in resultados)
                _pedidos.Add(pedido);
        }

        private async void LimpiarButton_Click(object sender, RoutedEventArgs e)
        {
            ExplotacionComboBox.SelectedIndex = -1;
            TipoSoporteComboBox.SelectedIndex = -1;
            await CargarTodosLosPedidosAsync();
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var createPedidoWindow = new CreatePedidoWindow(
                _pedidoService,
                _repuestoService,
                _tipoExplotacionService,
                _tipoRepuestoService,
                _tipoItemService,
                _itemService);

            if (createPedidoWindow.ShowDialog() == true)
                await CargarTodosLosPedidosAsync();
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedPedido = PedidosDataGrid.SelectedItem as Pedido;
            if (selectedPedido == null)
            {
                MessageBox.Show("Por favor, selecciona un pedido para editar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var editPedidoWindow = new CreatePedidoWindow(
                _pedidoService,
                _repuestoService,
                _tipoExplotacionService,
                _tipoRepuestoService,
                _tipoItemService,
                _itemService,
                selectedPedido);

            if (editPedidoWindow.ShowDialog() == true)
                await CargarTodosLosPedidosAsync();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedPedido = PedidosDataGrid.SelectedItem as Pedido;
            if (selectedPedido == null) return;

            if (MessageBox.Show("¿Estás seguro de que quieres eliminar este pedido?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                await _pedidoService.DeletePedidoAsync(selectedPedido.Id);
                await CargarTodosLosPedidosAsync();
            }
        }

        private async void EliminarRepuesto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int repuestoId)
            {
                if (MessageBox.Show("¿Estás seguro de que deseas eliminar este repuesto?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _repuestoService.DeleteRepuestoAsync(repuestoId);
                        MessageBox.Show("Repuesto eliminado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        await CargarTodosLosPedidosAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar el repuesto: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void PedidosDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Necesario para compilar XAML
        }

        private void ExportarExcelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isExporting) return;

            _isExporting = true;
            ExportarExcelButton.IsEnabled = false;

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Archivo de Excel (*.xlsx)|*.xlsx",
                FileName = "Pedidos.xlsx",
                OverwritePrompt = true
            };

            try
            {
                if (saveFileDialog.ShowDialog() == true)
                {
                    var file = new FileInfo(saveFileDialog.FileName);

                    using (var package = new ExcelPackage(file))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == "Pedidos_Unificado")
                            ?? package.Workbook.Worksheets.Add("Pedidos_Unificado");

                        worksheet.Cells.Clear();

                        // Encabezados
                        string[] headers = new string[]
                        {
                            "ID Pedido","Fecha Creación","Descripción Pedido","Incidencia","Fecha Incidencia","Fecha Llegada","Descripción Incidencia",
                            "ID Item","Ubicación","Tipo Soporte","Tipo Explotación",
                            "ID Repuesto","Nombre Repuesto","Cantidad","Descripción Repuesto","Precio","Tipo Repuesto"
                        };

                        for (int i = 0; i < headers.Length; i++)
                            worksheet.Cells[1, i + 1].Value = headers[i];

                        int row = 2;
                        foreach (var pedido in _pedidos)
                        {
                            foreach (var item in pedido.Items)
                            {
                                foreach (var repuesto in item.Repuestos)
                                {
                                    worksheet.Cells[row, 1].Value = pedido.Id;
                                    worksheet.Cells[row, 2].Value = pedido.FechaCreacion;
                                    worksheet.Cells[row, 2].Style.Numberformat.Format = "dd/MM/yyyy";
                                    worksheet.Cells[row, 3].Value = pedido.Descripcion;
                                    worksheet.Cells[row, 4].Value = pedido.Incidencia;
                                    worksheet.Cells[row, 5].Value = pedido.FechaIncidencia;
                                    worksheet.Cells[row, 5].Style.Numberformat.Format = "dd/MM/yyyy";
                                    worksheet.Cells[row, 6].Value = pedido.FechaLlegada;
                                    worksheet.Cells[row, 6].Style.Numberformat.Format = "dd/MM/yyyy";
                                    worksheet.Cells[row, 7].Value = pedido.DescripcionIncidencia;

                                    worksheet.Cells[row, 8].Value = item.Id;
                                    worksheet.Cells[row, 9].Value = item.NombreUbicacion;
                                    worksheet.Cells[row, 10].Value = item.TipoSoporte?.Nombre;
                                    worksheet.Cells[row, 11].Value = item.TipoExplotacion?.Nombre;

                                    worksheet.Cells[row, 12].Value = repuesto.Id;
                                    worksheet.Cells[row, 13].Value = repuesto.Nombre;
                                    worksheet.Cells[row, 14].Value = repuesto.Cantidad;
                                    worksheet.Cells[row, 15].Value = repuesto.Descripcion;
                                    worksheet.Cells[row, 16].Value = repuesto.Precio;
                                    worksheet.Cells[row, 17].Value = repuesto.TipoRepuesto?.Nombre;

                                    row++;
                                }
                            }
                        }

                        if (worksheet.Dimension != null)
                            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        package.Save();
                    }

                    MessageBox.Show("Datos exportados a Excel correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar a Excel: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isExporting = false;
                ExportarExcelButton.IsEnabled = true;
            }
        }
    }
}

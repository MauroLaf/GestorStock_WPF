using GestorStock.Data.Repositories;
using GestorStock.Model.Entities;
using GestorStock.Services.Implementations;
using GestorStock.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public MainWindow(IPedidoService pedidoService, IRepuestoService repuestoService, ITipoExplotacionService tipoExplotacionService, ITipoRepuestoService tipoRepuestoService, ITipoItemService tipoItemService, IItemService itemService)
        {
            InitializeComponent();
            _pedidoService = pedidoService;
            _repuestoService = repuestoService;
            _tipoExplotacionService = tipoExplotacionService;
            _tipoRepuestoService = tipoRepuestoService;
            _tipoItemService = tipoItemService;
            _itemService = itemService;

            // Inicializa la ObservableCollection y la asigna al DataGrid
            _pedidos = new ObservableCollection<Pedido>();
            PedidosDataGrid.ItemsSource = _pedidos;

            this.Loaded += MainWindow_Loaded;
            CreateButton.Click += CreateButton_Click;
            EditButton.Click += EditButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            BuscarButton.Click += BuscarButton_Click;
            LimpiarButton.Click += LimpiarButton_Click;
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
                // Limpia y rellena la colección para forzar la actualización completa del DataGrid
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
            {
                _pedidos.Add(pedido);
            }
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
            {
                await CargarTodosLosPedidosAsync();
            }
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
            {
                await CargarTodosLosPedidosAsync();
            }
        }

        [SupportedOSPlatform("windows")]
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedPedido = PedidosDataGrid.SelectedItem as Pedido;
            if (selectedPedido == null)
            {
                MessageBox.Show("Por favor, selecciona un pedido para eliminar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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
                var result = MessageBox.Show("¿Estás seguro de que deseas eliminar este repuesto?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
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
            // Este método puede quedar vacío
        }
    }
}
using GestorStock.Model.Entities;
using GestorStock.Services.Implementations;
using GestorStock.Services.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows;
using System; // Agregado para el manejo de excepciones

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
        private readonly IItemService _itemService; // Asegúrate de tener este servicio inyectado si lo necesitas en CreatePedidoWindow

        private ObservableCollection<Pedido> _pedidos;

        public MainWindow(IPedidoService pedidoService, IRepuestoService repuestoService, ITipoExplotacionService tipoExplotacionService, ITipoRepuestoService tipoRepuestoService, ITipoItemService tipoItemService, IItemService itemService) // Añade IItemService si lo usas
        {
            InitializeComponent();
            _pedidoService = pedidoService;
            _repuestoService = repuestoService;
            _tipoExplotacionService = tipoExplotacionService;
            _tipoRepuestoService = tipoRepuestoService;
            _tipoItemService = tipoItemService;
            _itemService = itemService; // Asigna el servicio de Item si lo usas

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
                ItemTypeComboBox.ItemsSource = tiposItem;
                ItemTypeComboBox.DisplayMemberPath = "Nombre";
                ItemTypeComboBox.SelectedIndex = -1;
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
                var pedidos = await _pedidoService.GetAllPedidosAsync();
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
            var tipoItemSeleccionado = ItemTypeComboBox.SelectedItem as TipoItem; // ¡Cambio aquí!

            var resultados = _pedidos.Where(p =>
                (explotacionSeleccionada == null || p.Items.Any(item => item.TipoExplotacion?.Id == explotacionSeleccionada.Id)) &&
                (tipoItemSeleccionado == null || p.Items.Any(item => item.TipoItem?.Id == tipoItemSeleccionado.Id)) // ¡Cambio aquí!
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
            ItemTypeComboBox.SelectedIndex = -1;
            await CargarTodosLosPedidosAsync();
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            // Pasa todos los servicios necesarios al constructor de CreatePedidoWindow
            var createPedidoWindow = new CreatePedidoWindow(
                _pedidoService,
                _itemService,
                _tipoExplotacionService,
                _repuestoService,
                _tipoRepuestoService,
                _tipoItemService);

            if (createPedidoWindow.ShowDialog() == true)
            {
                await CargarTodosLosPedidosAsync();
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Obtén el pedido seleccionado del DataGrid
            var selectedPedido = PedidosDataGrid.SelectedItem as Pedido;
            if (selectedPedido == null)
            {
                MessageBox.Show("Por favor, selecciona un pedido para editar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Pasa el pedido seleccionado y todos los servicios al constructor de la ventana
            var editPedidoWindow = new CreatePedidoWindow(
                _pedidoService,
                _itemService,
                _tipoExplotacionService,
                _repuestoService,
                _tipoRepuestoService,
                _tipoItemService,
                selectedPedido); // <-- ¡Pasa el pedido aquí!

            // Muestra la ventana de edición
            if (editPedidoWindow.ShowDialog() == true)
            {
                // Si el usuario guardó los cambios, recarga los pedidos
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

        private void PedidosDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
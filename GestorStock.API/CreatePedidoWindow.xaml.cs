using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Runtime.Versioning;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GestorStock.API
{
    [SupportedOSPlatform("windows")]
    public partial class CreatePedidoWindow : Window
    {
        private readonly IPedidoService _pedidoService;
        private readonly IItemService _itemService;
        private readonly ITipoExplotacionService _tipoExplotacionService;
        private readonly IRepuestoService _repuestoService;
        private readonly ITipoRepuestoService _tipoRepuestoService;
        private readonly ITipoItemService _tipoItemService;

        private ObservableCollection<Item> _items;
        private Pedido _pedido;

        public CreatePedidoWindow(
            // ORDEN CORREGIDO para que coincida con MainWindow
            IPedidoService pedidoService,
            IRepuestoService repuestoService,
            ITipoExplotacionService tipoExplotacionService,
            ITipoRepuestoService tipoRepuestoService,
            ITipoItemService tipoItemService,
            IItemService itemService,
            Pedido? pedidoToEdit = null)
        {
            InitializeComponent();
            _pedidoService = pedidoService;
            _itemService = itemService;
            _tipoExplotacionService = tipoExplotacionService;
            _repuestoService = repuestoService;
            _tipoRepuestoService = tipoRepuestoService;
            _tipoItemService = tipoItemService;

            _items = new ObservableCollection<Item>();
            ItemsDataGrid.ItemsSource = _items;

            if (pedidoToEdit != null)
            {
                _pedido = pedidoToEdit;
                this.Title = "Editar Pedido";
                LoadPedidoData();
            }
            else
            {
                _pedido = new Pedido
                {
                    FechaLlegada = DateTime.Now // <--- Cambio aquí
                };
            }

            this.Loaded += CreatePedidoWindow_Loaded;
            AddItemButton.Click += AddItemButton_Click;
            EditItemButton.Click += EditItemButton_Click;
            DeleteItemButton.Click += DeleteItemButton_Click;
            GuardarPedidoButton.Click += GuardarPedidoButton_Click;
            CancelarButton.Click += CancelarButton_Click;
        }

        private void CreatePedidoWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_pedido?.Items != null)
            {
                foreach (var item in _pedido.Items)
                {
                    _items.Add(item);
                }
            }
        }

        private void LoadPedidoData()
        {
            if (_pedido == null) return;
            DescripcionTextBox.Text = _pedido.Descripcion;
            IncidenciaCheckBox.IsChecked = _pedido.Incidencia;
            if (_pedido.FechaIncidencia.HasValue)
            {
                IncidenciaDatePicker.SelectedDate = _pedido.FechaIncidencia.Value;
            }
            DescripcionIncidenciaTextBox.Text = _pedido.DescripcionIncidencia;

            if (_pedido.Items != null)
            {
                _items.Clear();
                foreach (var item in _pedido.Items)
                {
                    _items.Add(item);
                }
            }
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            var addItemWindow = new AddItemWindow(
                _tipoExplotacionService,
                _repuestoService,
                _tipoRepuestoService,
                _tipoItemService);

            if (addItemWindow.ShowDialog() == true)
            {
                _items.Add(addItemWindow.ItemResult);
            }
        }

        private void EditItemButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = ItemsDataGrid.SelectedItem as Item;
            if (selectedItem == null)
            {
                MessageBox.Show("Por favor, selecciona un ítem para editar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var editItemWindow = new AddItemWindow(
                _tipoExplotacionService,
                _repuestoService,
                _tipoRepuestoService,
                _tipoItemService,
                selectedItem);

            if (editItemWindow.ShowDialog() == true)
            {
                var updatedItem = editItemWindow.ItemResult;

                var index = _items.IndexOf(selectedItem);
                if (index != -1)
                {
                    _items.RemoveAt(index);
                    _items.Insert(index, updatedItem);
                }
            }
        }

        private void DeleteItemButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = ItemsDataGrid.SelectedItem as Item;
            if (selectedItem == null)
            {
                MessageBox.Show("Por favor, selecciona un ítem para eliminar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _items.Remove(selectedItem);
        }

        private async void GuardarPedidoButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_items.Any())
            {
                MessageBox.Show("El pedido debe contener al menos un ítem.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _pedido.Descripcion = DescripcionTextBox.Text;
            _pedido.Incidencia = IncidenciaCheckBox.IsChecked ?? false;
            _pedido.FechaIncidencia = IncidenciaDatePicker.SelectedDate;
            _pedido.DescripcionIncidencia = DescripcionIncidenciaTextBox.Text;
            _pedido.Items = _items.ToList();

            if (_pedido.Id == 0)
            {
                await _pedidoService.CreatePedidoAsync(_pedido);
                MessageBox.Show("Pedido creado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                await _pedidoService.UpdatePedidoAsync(_pedido);
                MessageBox.Show("Pedido actualizado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            this.DialogResult = true;
        }

        private void CancelarButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
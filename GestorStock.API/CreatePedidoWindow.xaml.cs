using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Runtime.Versioning;
using System.Collections.Generic;

namespace GestorStock.API
{
    [SupportedOSPlatform("windows")]
    public partial class CreatePedidoWindow : Window
    {
        private readonly IPedidoService _pedidoService;
        private readonly IItemService _itemService;
        private readonly ITipoFamiliaService _tipoFamiliaService;
        private readonly IRepuestoService _repuestoService;
        private readonly ITipoRepuestoService _tipoRepuestoService;
        private readonly ITipoItemService _tipoItemService;
        private readonly IUbicacionProductoService _ubicacionProductoService;

        private ObservableCollection<Item> _items;
        public Pedido PedidoResult { get; private set; }

        public CreatePedidoWindow(
            IPedidoService pedidoService,
            IRepuestoService repuestoService,
            ITipoFamiliaService tipoFamiliaService,
            ITipoRepuestoService tipoRepuestoService,
            ITipoItemService tipoItemService,
            IItemService itemService,
            IUbicacionProductoService ubicacionProductoService,
            Pedido? pedidoToEdit = null)
        {
            InitializeComponent();
            _pedidoService = pedidoService;
            _itemService = itemService;
            _tipoFamiliaService = tipoFamiliaService;
            _repuestoService = repuestoService;
            _tipoRepuestoService = tipoRepuestoService;
            _tipoItemService = tipoItemService;
            _ubicacionProductoService = ubicacionProductoService;

            _items = new ObservableCollection<Item>();
            ItemsDataGrid.ItemsSource = _items;

            if (pedidoToEdit != null)
            {
                PedidoResult = pedidoToEdit;
                this.Title = "Editar Pedido";
                LoadPedidoData();
            }
            else
            {
                PedidoResult = new Pedido
                {
                    FechaCreacion = DateTime.Now
                };
            }

            AddItemButton.Click += AddItemButton_Click;
            EditItemButton.Click += EditItemButton_Click;
            DeleteItemButton.Click += DeleteItemButton_Click;
            GuardarPedidoButton.Click += GuardarPedidoButton_Click;
            CancelarButton.Click += CancelarButton_Click;
        }

        private void LoadPedidoData()
        {
            if (PedidoResult == null) return;
            DescripcionTextBox.Text = PedidoResult.Descripcion;
            IncidenciaCheckBox.IsChecked = PedidoResult.Incidencia;
            IncidenciaDatePicker.SelectedDate = PedidoResult.FechaIncidencia;
            FechaLlegadaDatePicker.SelectedDate = PedidoResult.FechaLlegada;
            DescripcionIncidenciaTextBox.Text = PedidoResult.DescripcionIncidencia;

            if (PedidoResult.Items != null)
            {
                _items.Clear();
                foreach (var item in PedidoResult.Items)
                {
                    _items.Add(item);
                }
            }
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            var addItemWindow = new AddItemWindow(
                _tipoFamiliaService,
                _repuestoService,
                _tipoRepuestoService,
                _tipoItemService,
                _ubicacionProductoService);

            if (addItemWindow.ShowDialog() == true)
            {
                _items.Add(addItemWindow.ItemResult);
            }
        }

        private async void EditItemButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = ItemsDataGrid.SelectedItem as Item;
            if (selectedItem == null)
            {
                MessageBox.Show("Por favor, selecciona un ítem para editar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Item? itemToEdit;

            // Si el ítem no tiene ID (es nuevo, no guardado en BD), usa el objeto directamente
            if (selectedItem.Id == 0)
            {
                itemToEdit = selectedItem;
            }
            else
            {
                // Si tiene ID, cárgalo desde la base de datos con todas sus relaciones
                try
                {
                    itemToEdit = await _itemService.GetItemWithAllRelationsAsync(selectedItem.Id);
                }

                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar el ítem para editar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (itemToEdit == null)
            {
                MessageBox.Show("No se pudo cargar el ítem para editar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var editItemWindow = new AddItemWindow(
                _tipoFamiliaService,
                _repuestoService,
                _tipoRepuestoService,
                _tipoItemService,
                _ubicacionProductoService,
                itemToEdit);

            if (editItemWindow.ShowDialog() == true)
            {
                var index = _items.IndexOf(selectedItem);
                if (index != -1)
                {
                    // Usar RemoveAt e Insert en lugar de asignación directa
                    // para que ObservableCollection notifique el cambio correctamente
                    _items.RemoveAt(index);
                    _items.Insert(index, editItemWindow.ItemResult);
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
            PedidoResult.Descripcion = DescripcionTextBox.Text;
            PedidoResult.Incidencia = IncidenciaCheckBox.IsChecked ?? false;
            PedidoResult.FechaIncidencia = IncidenciaDatePicker.SelectedDate;
            PedidoResult.FechaLlegada = FechaLlegadaDatePicker.SelectedDate;
            PedidoResult.DescripcionIncidencia = DescripcionIncidenciaTextBox.Text;
            PedidoResult.Items = _items.ToList();

            if (PedidoResult.Id == 0)
            {
                await _pedidoService.CreatePedidoAsync(PedidoResult);
                MessageBox.Show("Pedido creado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                await _pedidoService.UpdatePedidoAsync(PedidoResult);
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
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Runtime.Versioning;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.Generic;

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
        public Pedido PedidoResult { get; private set; }

        public CreatePedidoWindow(
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
                ItemsDataGrid.Items.Refresh();
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
using GestorStock.Data.Repositories;
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Runtime.Versioning;
using System;

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
        private Pedido? _pedidoToEdit;

        // Constructor para CREAR un nuevo pedido
        public CreatePedidoWindow(IPedidoService pedidoService, IItemService itemService, ITipoExplotacionService tipoExplotacionService, IRepuestoService repuestoService, ITipoRepuestoService tipoRepuestoService, ITipoItemService tipoItemService)
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

            this.Loaded += CreatePedidoWindow_Loaded;
            AddItemButton.Click += AddItemButton_Click;
            EditItemButton.Click += EditItemButton_Click; // ¡Agrega este evento!
            AcceptButton.Click += AcceptButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        // Constructor para EDITAR un pedido existente
        public CreatePedidoWindow(IPedidoService pedidoService, IItemService itemService, ITipoExplotacionService tipoExplotacionService, IRepuestoService repuestoService, ITipoRepuestoService tipoRepuestoService, ITipoItemService tipoItemService, Pedido pedidoToEdit)
            : this(pedidoService, itemService, tipoExplotacionService, repuestoService, tipoRepuestoService, tipoItemService)
        {
            _pedidoToEdit = pedidoToEdit;
            this.Title = "Editar Pedido";
            LoadPedidoData();
        }

        private void CreatePedidoWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Puedes cargar datos iniciales aquí si es necesario
        }

        private void LoadPedidoData()
        {
            if (_pedidoToEdit == null) return;
            DescripcionTextBox.Text = _pedidoToEdit.Descripcion;
            IncidenciaCheckBox.IsChecked = _pedidoToEdit.Incidencia;
            if (_pedidoToEdit.FechaIncidencia.HasValue)
            {
                IncidenciaDatePicker.SelectedDate = _pedidoToEdit.FechaIncidencia.Value;
            }
            DescripcionIncidenciaTextBox.Text = _pedidoToEdit.DescripcionIncidencia;

            if (_pedidoToEdit.Items != null)
            {
                _items.Clear();
                foreach (var item in _pedidoToEdit.Items)
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
                var newItem = addItemWindow.GetItem();
                if (newItem != null)
                {
                    _items.Add(newItem);
                }
            }
        }

        // Nuevo método para editar un ítem
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
                selectedItem); // Se pasa el ítem para que lo edite

            if (editItemWindow.ShowDialog() == true)
            {
                // La ObservableCollection ya se encargará de notificar los cambios
                // al DataGrid, pero si no se actualiza, puedes forzar el refresh
                ItemsDataGrid.Items.Refresh();
            }
        }

        private async void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_items.Any())
            {
                MessageBox.Show("Debe agregar al menos un ítem al pedido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_pedidoToEdit != null)
            {
                _pedidoToEdit.Descripcion = DescripcionTextBox.Text;
                _pedidoToEdit.Incidencia = IncidenciaCheckBox.IsChecked ?? false;
                _pedidoToEdit.FechaIncidencia = IncidenciaDatePicker.SelectedDate;
                _pedidoToEdit.DescripcionIncidencia = DescripcionIncidenciaTextBox.Text;
                _pedidoToEdit.Items = _items.ToList();

                await _pedidoService.UpdatePedidoAsync(_pedidoToEdit);
                MessageBox.Show("Pedido actualizado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                var newPedido = new Pedido
                {
                    Fecha = DateTime.Today,
                    Descripcion = DescripcionTextBox.Text,
                    Incidencia = IncidenciaCheckBox.IsChecked ?? false,
                    FechaIncidencia = IncidenciaDatePicker.SelectedDate,
                    DescripcionIncidencia = DescripcionIncidenciaTextBox.Text,
                    Items = _items.ToList()
                };

                await _pedidoService.CreatePedidoAsync(newPedido);
                MessageBox.Show("Pedido creado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
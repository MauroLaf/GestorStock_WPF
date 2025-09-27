using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Runtime.Versioning;
using System;
using System.Collections.Generic;

namespace GestorStock.API
{
    [SupportedOSPlatform("windows")]
    public partial class AddItemWindow : Window
    {
        private readonly ITipoExplotacionService _tipoExplotacionService;
        private readonly IRepuestoService _repuestoService;
        private readonly ITipoRepuestoService _tipoRepuestoService;
        private readonly ITipoItemService _tipoItemService;

        public ObservableCollection<Repuesto> _repuestos;
        public Item ItemResult { get; private set; }

        public AddItemWindow(
            ITipoExplotacionService tipoExplotacionService,
            IRepuestoService repuestoService,
            ITipoRepuestoService tipoRepuestoService,
            ITipoItemService tipoItemService,
            Item? itemToEdit = null)
        {
            InitializeComponent();
            _tipoExplotacionService = tipoExplotacionService;
            _repuestoService = repuestoService;
            _tipoRepuestoService = tipoRepuestoService;
            _tipoItemService = tipoItemService;

            ItemResult = itemToEdit ?? new Item();
            this.Title = itemToEdit != null ? "Editar Ítem" : "Agregar Ítem";

            _repuestos = new ObservableCollection<Repuesto>(ItemResult.Repuestos ?? new List<Repuesto>());
            RepuestosDataGrid.ItemsSource = _repuestos;

            this.Loaded += AddItemWindow_Loaded;
            AddRepuestoButton.Click += AddRepuestoButton_Click;
            EditRepuestoButton.Click += EditRepuestoButton_Click;
            DeleteRepuestoButton.Click += DeleteRepuestoButton_Click;
            AceptarButton.Click += AceptarButton_Click;
            CancelarButton.Click += CancelarButton_Click;
            CantidadTextBox.PreviewTextInput += CantidadTextBox_PreviewTextInput;
            if (PrecioTextBox != null)
            {
                PrecioTextBox.PreviewTextInput += PrecioTextBox_PreviewTextInput;
            }
        }

        private async void AddItemWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                TipoExplotacionComboBox.ItemsSource = await _tipoExplotacionService.GetAllTipoExplotacionAsync();
                TipoSoporteComboBox.ItemsSource = await _tipoItemService.GetAllTipoItemAsync();
                TipoRepuestoComboBox.ItemsSource = await _tipoRepuestoService.GetAllTipoRepuestoAsync();

                if (ItemResult.Id != 0 || !string.IsNullOrEmpty(ItemResult.NombreUbicacion))
                {
                    LoadItemData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.DialogResult = false;
                this.Close();
            }
        }

        private void LoadItemData()
        {
            NombreUbicacionTextBox.Text = ItemResult.NombreUbicacion;

            if (ItemResult.TipoExplotacionId != 0)
            {
                TipoExplotacionComboBox.SelectedItem = (TipoExplotacionComboBox.ItemsSource as IEnumerable<TipoExplotacion>)?.FirstOrDefault(t => t.Id == ItemResult.TipoExplotacionId);
            }

            if (ItemResult.TipoItemId != 0)
            {
                TipoSoporteComboBox.SelectedItem = (TipoSoporteComboBox.ItemsSource as IEnumerable<TipoSoporte>)?.FirstOrDefault(t => t.Id == ItemResult.TipoItemId);
            }

            _repuestos.Clear();
            if (ItemResult.Repuestos != null)
            {
                foreach (var repuesto in ItemResult.Repuestos)
                {
                    _repuestos.Add(repuesto);
                }
            }
        }

        private void AddRepuestoButton_Click(object sender, RoutedEventArgs e)
        {
            int cantidad = 0;
            TipoRepuesto? selectedTipoRepuesto = null;
            decimal precio = 0;

            if (string.IsNullOrWhiteSpace(RepuestoTextBox.Text) || string.IsNullOrWhiteSpace(CantidadTextBox.Text))
            {
                MessageBox.Show("Debe ingresar un nombre para el repuesto y la cantidad.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(CantidadTextBox.Text, out cantidad) || cantidad <= 0)
            {
                MessageBox.Show("La cantidad debe ser un número entero positivo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(PrecioTextBox.Text))
            {
                MessageBox.Show("Debe ingresar el precio del repuesto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!decimal.TryParse(PrecioTextBox.Text, out precio) || precio < 0)
            {
                MessageBox.Show("El precio debe ser un número válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            selectedTipoRepuesto = TipoRepuestoComboBox.SelectedItem as TipoRepuesto;
            if (selectedTipoRepuesto == null)
            {
                MessageBox.Show("Debe seleccionar un tipo de repuesto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var existingRepuesto = _repuestos.FirstOrDefault(r => r.Nombre.Equals(RepuestoTextBox.Text, StringComparison.OrdinalIgnoreCase));
            if (existingRepuesto != null)
            {
                existingRepuesto.Cantidad = cantidad;
                existingRepuesto.Precio = precio;
                existingRepuesto.TipoRepuesto = selectedTipoRepuesto;
                existingRepuesto.TipoRepuestoId = selectedTipoRepuesto.Id;
            }
            else
            {
                var newRepuesto = new Repuesto
                {
                    Nombre = RepuestoTextBox.Text,
                    Cantidad = cantidad,
                    Precio = precio,
                    TipoRepuesto = selectedTipoRepuesto,
                    TipoRepuestoId = selectedTipoRepuesto.Id
                };
                _repuestos.Add(newRepuesto);
            }

            RepuestosDataGrid.Items.Refresh();
            LimpiarCamposRepuesto();
        }

        private void EditRepuestoButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRepuesto = RepuestosDataGrid.SelectedItem as Repuesto;
            if (selectedRepuesto == null)
            {
                MessageBox.Show("Por favor, selecciona un repuesto para editar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Aquí se cargan los valores del repuesto seleccionado a los campos de texto y combobox.
            RepuestoTextBox.Text = selectedRepuesto.Nombre;
            CantidadTextBox.Text = selectedRepuesto.Cantidad.ToString();
            PrecioTextBox.Text = selectedRepuesto.Precio.ToString();

            // Esto es crucial: se busca el TipoRepuesto por su ID para garantizar que se seleccione el objeto correcto del ComboBox.
            if (selectedRepuesto.TipoRepuestoId.HasValue)
            {
                TipoRepuestoComboBox.SelectedItem = (TipoRepuestoComboBox.ItemsSource as IEnumerable<TipoRepuesto>)?.FirstOrDefault(t => t.Id == selectedRepuesto.TipoRepuestoId.Value);
            }
            else
            {
                TipoRepuestoComboBox.SelectedItem = null;
            }
        }

        private void DeleteRepuestoButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRepuesto = RepuestosDataGrid.SelectedItem as Repuesto;
            if (selectedRepuesto == null)
            {
                MessageBox.Show("Por favor, selecciona un repuesto para eliminar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _repuestos.Remove(selectedRepuesto);
        }

        private void AceptarButton_Click(object sender, RoutedEventArgs e)
        {
            ItemResult.NombreUbicacion = NombreUbicacionTextBox.Text;

            // Se asigna la relación completa del objeto, no solo el ID.
            ItemResult.TipoExplotacion = TipoExplotacionComboBox.SelectedItem as TipoExplotacion;
            ItemResult.TipoExplotacionId = ItemResult.TipoExplotacion?.Id ?? 0;

            // Se asigna la relación completa del objeto, no solo el ID.
            ItemResult.TipoSoporte = TipoSoporteComboBox.SelectedItem as TipoSoporte;
            ItemResult.TipoItemId = ItemResult.TipoSoporte?.Id ?? 0;

            // Asigna la colección de repuestos a la propiedad del ítem.
            ItemResult.Repuestos = _repuestos.ToList();

            this.DialogResult = true;
        }

        private void CancelarButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void CantidadTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PrecioTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void LimpiarCamposRepuesto()
        {
            RepuestoTextBox.Clear();
            CantidadTextBox.Clear();
            PrecioTextBox.Clear();
            TipoRepuestoComboBox.SelectedItem = null;
        }
    }
}
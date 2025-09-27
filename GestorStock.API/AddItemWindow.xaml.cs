using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Runtime.Versioning;
using System.Threading.Tasks;
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

        private bool _isEditingRepuesto = false;

        // Constructor para CREAR un nuevo ítem
        public AddItemWindow(ITipoExplotacionService tipoExplotacionService, IRepuestoService repuestoService, ITipoRepuestoService tipoRepuestoService, ITipoItemService tipoItemService)
        {
            InitializeComponent();
            _tipoExplotacionService = tipoExplotacionService;
            _repuestoService = repuestoService;
            _tipoRepuestoService = tipoRepuestoService;
            _tipoItemService = tipoItemService;

            ItemResult = new Item();
            _repuestos = new ObservableCollection<Repuesto>();
            RepuestosDataGrid.ItemsSource = _repuestos;

            this.Loaded += AddItemWindow_Loaded;
            AddRepuestoButton.Click += AddRepuestoButton_Click;
            EditRepuestoButton.Click += EditRepuestoButton_Click;
            DeleteRepuestoButton.Click += DeleteRepuestoButton_Click;
            AceptarButton.Click += AceptarButton_Click;
            CancelarButton.Click += CancelarButton_Click;
            CantidadTextBox.PreviewTextInput += CantidadTextBox_PreviewTextInput;
            // Asegúrate de que este evento esté en tu XAML
            // <TextBox Name="PrecioTextBox" PreviewTextInput="PrecioTextBox_PreviewTextInput"/>
            if (PrecioTextBox != null)
            {
                PrecioTextBox.PreviewTextInput += PrecioTextBox_PreviewTextInput;
            }
        }

        // Constructor para EDITAR un ítem existente
        public AddItemWindow(ITipoExplotacionService tipoExplotacionService, IRepuestoService repuestoService, ITipoRepuestoService tipoRepuestoService, ITipoItemService tipoItemService, Item itemToEdit)
            : this(tipoExplotacionService, repuestoService, tipoRepuestoService, tipoItemService)
        {
            ItemResult = itemToEdit;
            this.Title = "Editar Ítem";
        }

        private async void AddItemWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                TipoExplotacionComboBox.ItemsSource = await _tipoExplotacionService.GetAllTipoExplotacionAsync();
                TipoSoporteComboBox.ItemsSource = await _tipoItemService.GetAllTipoItemAsync();
                TipoRepuestoComboBox.ItemsSource = await _tipoRepuestoService.GetAllTipoRepuestoAsync();

                if (ItemResult.Id != 0)
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

            TipoExplotacionComboBox.SelectedItem = (TipoExplotacionComboBox.ItemsSource as IEnumerable<TipoExplotacion>)?.FirstOrDefault(t => t.Id == ItemResult.TipoExplotacionId);
            TipoSoporteComboBox.SelectedItem = (TipoSoporteComboBox.ItemsSource as IEnumerable<TipoSoporte>)?.FirstOrDefault(t => t.Id == ItemResult.TipoItemId);

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
            // Mover las declaraciones al inicio del método para corregir el error de alcance
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

            // Lógica para agregar o editar
            if (_isEditingRepuesto)
            {
                var repuestoToEdit = RepuestosDataGrid.SelectedItem as Repuesto;
                if (repuestoToEdit != null)
                {
                    repuestoToEdit.Nombre = RepuestoTextBox.Text;
                    repuestoToEdit.Cantidad = cantidad;
                    repuestoToEdit.Precio = precio;
                    repuestoToEdit.TipoRepuesto = selectedTipoRepuesto;
                    repuestoToEdit.TipoRepuestoId = selectedTipoRepuesto.Id;
                }
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

            // Restablecer el estado
            RepuestoTextBox.Clear();
            CantidadTextBox.Clear();
            PrecioTextBox.Clear();
            TipoRepuestoComboBox.SelectedIndex = -1;
            AddRepuestoButton.Content = "Agregar Repuesto";
            _isEditingRepuesto = false;
        }

        private void EditRepuestoButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRepuesto = RepuestosDataGrid.SelectedItem as Repuesto;
            if (selectedRepuesto == null)
            {
                MessageBox.Show("Por favor, selecciona un repuesto para editar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            RepuestoTextBox.Text = selectedRepuesto.Nombre;
            CantidadTextBox.Text = selectedRepuesto.Cantidad.ToString();
            // Asegúrate de que esta línea esté presente
            PrecioTextBox.Text = selectedRepuesto.Precio.ToString();

            if (selectedRepuesto.TipoRepuestoId != null)
            {
                TipoRepuestoComboBox.SelectedItem = (TipoRepuestoComboBox.ItemsSource as IEnumerable<TipoRepuesto>)?.FirstOrDefault(t => t.Id == selectedRepuesto.TipoRepuestoId);
            }

            AddRepuestoButton.Content = "Guardar Cambios";
            _isEditingRepuesto = true;
        }

        private void DeleteRepuestoButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRepuesto = RepuestosDataGrid.SelectedItem as Repuesto;
            if (selectedRepuesto == null)
            {
                MessageBox.Show("Por favor, selecciona un repuesto para eliminar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show(
                $"¿Estás seguro de que deseas eliminar el repuesto '{selectedRepuesto.Nombre}' de la lista?",
                "Confirmar Eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _repuestos.Remove(selectedRepuesto);
                MessageBox.Show("Repuesto eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AceptarButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTipoExplotacion = TipoExplotacionComboBox.SelectedItem as TipoExplotacion;
            var selectedTipoSoporte = TipoSoporteComboBox.SelectedItem as TipoSoporte;

            if (selectedTipoExplotacion == null || selectedTipoSoporte == null)
            {
                MessageBox.Show("Debe seleccionar un tipo de explotación y un tipo de ítem válidos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(NombreUbicacionTextBox.Text))
            {
                MessageBox.Show("Debe ingresar un nombre para el ítem.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!_repuestos.Any())
            {
                MessageBox.Show("Debe agregar al menos un repuesto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ItemResult.TipoExplotacion = selectedTipoExplotacion;
            ItemResult.TipoExplotacionId = selectedTipoExplotacion.Id;
            ItemResult.TipoSoporte = selectedTipoSoporte;
            ItemResult.TipoItemId = selectedTipoSoporte.Id;
            ItemResult.NombreUbicacion = NombreUbicacionTextBox.Text;
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
            Regex regex = new Regex("^[0-9]+([,][0-9]{1,2})?$");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}
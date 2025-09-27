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
        }

        // Constructor para EDITAR un ítem existente
        public AddItemWindow(ITipoExplotacionService tipoExplotacionService, IRepuestoService repuestoService, ITipoRepuestoService tipoRepuestoService, ITipoItemService tipoItemService, Item itemToEdit)
            : this(tipoExplotacionService, repuestoService, tipoRepuestoService, tipoItemService)
        {
            ItemResult = itemToEdit;
            this.Title = "Editar Ítem";
            // La carga de datos se manejará en el evento Loaded
        }

        private async void AddItemWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Carga los ComboBox de forma más simple
                TipoExplotacionComboBox.ItemsSource = await _tipoExplotacionService.GetAllTipoExplotacionAsync();
                TipoSoporteComboBox.ItemsSource = await _tipoItemService.GetAllTipoItemAsync();
                TipoRepuestoComboBox.ItemsSource = await _tipoRepuestoService.GetAllTipoRepuestoAsync(); // Carga de tipos de repuesto

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

            // Acceso seguro con ?
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
            // Se usa RepuestoTextBox en lugar de RepuestoComboBox
            if (string.IsNullOrWhiteSpace(RepuestoTextBox.Text) || string.IsNullOrWhiteSpace(CantidadTextBox.Text))
            {
                MessageBox.Show("Debe ingresar un nombre para el repuesto y la cantidad.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(CantidadTextBox.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("La cantidad debe ser un número entero positivo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            // Se usa TipoRepuestoComboBox para obtener el tipo
            TipoRepuesto? selectedTipoRepuesto = TipoRepuestoComboBox.SelectedItem as TipoRepuesto;
            if (selectedTipoRepuesto == null)
            {
                 MessageBox.Show("Debe seleccionar un tipo de repuesto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                 return;
            }


            if (_isEditingRepuesto)
            {
                var repuestoToEdit = RepuestosDataGrid.SelectedItem as Repuesto;
                if (repuestoToEdit != null)
                {
                    repuestoToEdit.Nombre = RepuestoTextBox.Text; // Usa el texto del TextBox
                    repuestoToEdit.Cantidad = cantidad;
                    repuestoToEdit.TipoRepuesto = selectedTipoRepuesto;
                    repuestoToEdit.TipoRepuestoId = selectedTipoRepuesto.Id;
                    RepuestosDataGrid.Items.Refresh();
                }
                AddRepuestoButton.Content = "Agregar Repuesto";
                _isEditingRepuesto = false;
            }
            else
            {
                var newRepuesto = new Repuesto
                {
                    Nombre = RepuestoTextBox.Text, // Usa el texto del TextBox
                    Cantidad = cantidad,
                    TipoRepuesto = selectedTipoRepuesto,
                    TipoRepuestoId = selectedTipoRepuesto.Id
                };
                _repuestos.Add(newRepuesto);
            }

            RepuestoTextBox.Clear();
            CantidadTextBox.Clear();
            TipoRepuestoComboBox.SelectedIndex = -1;
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
            
            // Si el repuesto tiene un tipo, lo selecciona
            if(selectedRepuesto.TipoRepuestoId != null)
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
    }
}
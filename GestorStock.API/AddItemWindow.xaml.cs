using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GestorStock.API
{
    [SupportedOSPlatform("windows")]
    public partial class AddItemWindow : Window
    {
        private readonly ITipoFamiliaService _tipoFamiliaService;
        private readonly ITipoRepuestoService _tipoRepuestoService;
        private readonly ITipoItemService _tipoItemService;
        private readonly IUbicacionProductoService _ubicacionProductoService;

        public ObservableCollection<Repuesto> Repuestos { get; private set; }
        public Item ItemResult { get; private set; }
        private Repuesto? _repuestoToEdit;
        private bool _isEditingItem = false;

        public AddItemWindow(
            ITipoFamiliaService tipoFamiliaService,
            IRepuestoService repuestoService,
            ITipoRepuestoService tipoRepuestoService,
            ITipoItemService tipoItemService,
            IUbicacionProductoService ubicacionProductoService,
            Item? itemToEdit = null)
        {
            InitializeComponent();
            _tipoFamiliaService = tipoFamiliaService;
            _tipoRepuestoService = tipoRepuestoService;
            _tipoItemService = tipoItemService;
            _ubicacionProductoService = ubicacionProductoService;

            _isEditingItem = itemToEdit != null;
            ItemResult = itemToEdit ?? new Item();

            this.DataContext = ItemResult;

            this.Title = _isEditingItem ? "Editar Ítem" : "Agregar Ítem";

            Repuestos = new ObservableCollection<Repuesto>(ItemResult.Repuestos ?? new List<Repuesto>());
            RepuestosDataGrid.ItemsSource = Repuestos;

            this.Loaded += AddItemWindow_Loaded;
            AddUbicacionProductoButton.Click += AddUbicacionProductoButton_Click;

            // ⭐ Conexión del botón de Quitar Ubicación (Signo -) ⭐
            RemoveUbicacionProductoButton.Click += RemoveUbicacionProductoButton_Click;

            AddRepuestoButton.Click += AddRepuestoButton_Click;
            EditRepuestoButton.Click += EditRepuestoButton_Click;
            UpdateRepuestoButton.Click += UpdateRepuestoButton_Click;
            DeleteRepuestoButton.Click += DeleteRepuestoButton_Click;
            AceptarButton.Click += AceptarButton_Click;
            CancelarButton.Click += CancelarButton_Click;
            CantidadTextBox.PreviewTextInput += CantidadTextBox_PreviewTextInput;
            PrecioTextBox.PreviewTextInput += PrecioTextBox_PreviewTextInput;
            TipoFamiliaComboBox.SelectionChanged += TipoFamiliaComboBox_SelectionChanged;
        }

        private async void AddItemWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Cargar todas las listas de opciones
                TipoFamiliaComboBox.ItemsSource = await _tipoFamiliaService.GetAllTipoFamiliaAsync();
                TipoSoporteComboBox.ItemsSource = await _tipoItemService.GetAllTipoItemAsync();
                TipoRepuestoComboBox.ItemsSource = await _tipoRepuestoService.GetAllTipoRepuestoAsync();

                if (_isEditingItem && ItemResult != null)
                {
                    // PRIMERO: Seleccionar la familia para que se carguen las ubicaciones correctas
                    if (ItemResult.FamiliaId.HasValue)
                    {
                        var familia = (TipoFamiliaComboBox.ItemsSource as IEnumerable<Familia>)?
                                             .FirstOrDefault(f => f.Id == ItemResult.FamiliaId.Value);

                        if (familia != null)
                        {
                            // Desactivar temporalmente el evento para evitar que se limpie la selección
                            TipoFamiliaComboBox.SelectionChanged -= TipoFamiliaComboBox_SelectionChanged;
                            TipoFamiliaComboBox.SelectedItem = familia;
                            TipoFamiliaComboBox.SelectionChanged += TipoFamiliaComboBox_SelectionChanged;

                            // Cargar ubicaciones de esta familia
                            await LoadUbicacionProductosByFamilia();
                        }
                    }

                    // SEGUNDO: Ahora seleccionar la ubicación (después de que se hayan cargado)
                    if (ItemResult.UbicacionProductoId.HasValue)
                    {
                        var ubicacion = (UbicacionProductoComboBox.ItemsSource as IEnumerable<UbicacionProducto>)?
                                                 .FirstOrDefault(up => up.Id == ItemResult.UbicacionProductoId.Value);
                        UbicacionProductoComboBox.SelectedItem = ubicacion;
                    }

                    // TERCERO: Seleccionar el tipo de soporte
                    if (ItemResult.TipoSoporteId.HasValue)
                    {
                        var tipoSoporte = (TipoSoporteComboBox.ItemsSource as IEnumerable<TipoSoporte>)?
                                                     .FirstOrDefault(s => s.Id == ItemResult.TipoSoporteId.Value);
                        TipoSoporteComboBox.SelectedItem = tipoSoporte;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.DialogResult = false;
                this.Close();
            }
        }

        private async void TipoFamiliaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await LoadUbicacionProductosByFamilia();

            // Solo limpiar si NO estamos en modo edición o si ya terminó de cargar
            if (!_isEditingItem || UbicacionProductoComboBox.ItemsSource != null)
            {
                UbicacionProductoComboBox.SelectedItem = null;
                ItemResult.UbicacionProducto = null;
                ItemResult.UbicacionProductoId = null;
            }
        }

        private async Task LoadUbicacionProductosByFamilia()
        {
            var selectedFamilia = TipoFamiliaComboBox.SelectedItem as Familia;
            if (selectedFamilia != null)
            {
                UbicacionProductoComboBox.ItemsSource = await _ubicacionProductoService.GetUbicacionProductosByFamiliaIdAsync(selectedFamilia.Id);
            }
            else
            {
                UbicacionProductoComboBox.ItemsSource = null;
            }
        }

        private async void AddUbicacionProductoButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedFamilia = TipoFamiliaComboBox.SelectedItem as Familia;
            if (selectedFamilia == null)
            {
                MessageBox.Show("Por favor, selecciona primero una familia de ubicación.", "Selección requerida", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var newLocationName = Microsoft.VisualBasic.Interaction.InputBox(
                "Ingresa el nombre de la nueva ubicación:",
                "Agregar Ubicación",
                "");

            if (!string.IsNullOrWhiteSpace(newLocationName))
            {
                try
                {
                    var newUbicacion = new UbicacionProducto
                    {
                        Nombre = newLocationName,
                        FamiliaId = selectedFamilia.Id
                        // Se omite la asignación de la entidad Familia para evitar el error de clave duplicada
                    };

                    await _ubicacionProductoService.CreateUbicacionProductoAsync(newUbicacion);
                    await LoadUbicacionProductosByFamilia();
                    UbicacionProductoComboBox.SelectedItem = newUbicacion;

                    MessageBox.Show("Ubicación añadida correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    // Mostrar la excepción interna para un diagnóstico preciso
                    string errorMessage = $"Error al agregar la ubicación: {ex.Message}";

                    if (ex.InnerException != null)
                    {
                        errorMessage += $"\n\nDetalle de la Base de Datos: {ex.InnerException.Message}";
                    }

                    MessageBox.Show(errorMessage, "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // ⭐ MÉTODO CORREGIDO PARA ELIMINAR LA ENTIDAD DE LA BASE DE DATOS ⭐
        private async void RemoveUbicacionProductoButton_Click(object sender, RoutedEventArgs e)
        {
            var ubicacionToDelete = UbicacionProductoComboBox.SelectedItem as UbicacionProducto;

            if (ubicacionToDelete == null)
            {
                MessageBox.Show("Por favor, selecciona una Ubicación/Producto de la lista para eliminarla.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"¿Estás seguro de que quieres eliminar la ubicación '{ubicacionToDelete.Nombre}'?\n\nAdvertencia: Esto la eliminará permanentemente de la base de datos y de todos los ítems asociados que no estén guardados.",
                "Confirmar Eliminación de Ubicación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Llama al servicio para eliminar la entidad de la base de datos
                    await _ubicacionProductoService.DeleteUbicacionProductoAsync(ubicacionToDelete.Id);

                    // Recarga la lista de ubicaciones para que el ComboBox se actualice
                    await LoadUbicacionProductosByFamilia();

                    // Limpia la selección en el formulario actual (Importante)
                    UbicacionProductoComboBox.SelectedItem = null;
                    ItemResult.UbicacionProducto = null;
                    ItemResult.UbicacionProductoId = null;

                    MessageBox.Show("Ubicación eliminada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    string errorMessage = $"Error al eliminar la ubicación: {ex.Message}";

                    if (ex.InnerException != null)
                    {
                        errorMessage += $"\n\nDetalle de la Base de Datos: {ex.InnerException.Message}";
                    }
                    // Esto suele ocurrir si la ubicación está siendo utilizada por otro registro (violación de clave foránea)
                    MessageBox.Show(errorMessage, "Error de Eliminación", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AceptarButton_Click(object sender, RoutedEventArgs e)
        {
            // Actualizar el ItemResult con los valores de los ComboBoxes seleccionados
            ItemResult.Familia = TipoFamiliaComboBox.SelectedItem as Familia;
            ItemResult.FamiliaId = ItemResult.Familia?.Id;

            ItemResult.UbicacionProducto = UbicacionProductoComboBox.SelectedItem as UbicacionProducto;
            ItemResult.UbicacionProductoId = ItemResult.UbicacionProducto?.Id;

            ItemResult.TipoSoporte = TipoSoporteComboBox.SelectedItem as TipoSoporte;
            ItemResult.TipoSoporteId = ItemResult.TipoSoporte?.Id;

            ItemResult.Repuestos = Repuestos.ToList();

            this.DialogResult = true;
            this.Close();
        }

        private void CancelarButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void AddRepuestoButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateRepuestoInputs(out int cantidad, out decimal precio, out TipoRepuesto? selectedTipoRepuesto)) return;

            var existingRepuesto = Repuestos.FirstOrDefault(r => r.Nombre.Equals(RepuestoTextBox.Text, StringComparison.OrdinalIgnoreCase));
            if (existingRepuesto != null)
            {
                MessageBox.Show("Ya existe un repuesto con ese nombre. Use la función de 'Editar' para modificarlo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newRepuesto = new Repuesto
            {
                Nombre = RepuestoTextBox.Text,
                Cantidad = cantidad,
                Precio = precio,
                TipoRepuesto = selectedTipoRepuesto,
                TipoRepuestoId = selectedTipoRepuesto?.Id
            };
            Repuestos.Add(newRepuesto);

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
            _repuestoToEdit = selectedRepuesto;
            RepuestoTextBox.Text = selectedRepuesto.Nombre;
            CantidadTextBox.Text = selectedRepuesto.Cantidad.ToString();
            PrecioTextBox.Text = selectedRepuesto.Precio.ToString();

            TipoRepuestoComboBox.SelectedItem = (TipoRepuestoComboBox.ItemsSource as IEnumerable<TipoRepuesto>)?.FirstOrDefault(t => t.Id == selectedRepuesto.TipoRepuestoId);

            AddRepuestoButton.Visibility = Visibility.Collapsed;
            UpdateRepuestoButton.Visibility = Visibility.Visible;
        }

        private void UpdateRepuestoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_repuestoToEdit == null)
            {
                MessageBox.Show("No se ha seleccionado ningún repuesto para actualizar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!ValidateRepuestoInputs(out int cantidad, out decimal precio, out TipoRepuesto? selectedTipoRepuesto)) return;

            _repuestoToEdit.Nombre = RepuestoTextBox.Text;
            _repuestoToEdit.Cantidad = cantidad;
            _repuestoToEdit.Precio = precio;
            _repuestoToEdit.TipoRepuesto = selectedTipoRepuesto;
            _repuestoToEdit.TipoRepuestoId = selectedTipoRepuesto?.Id;

            RepuestosDataGrid.Items.Refresh();
            AddRepuestoButton.Visibility = Visibility.Visible;
            UpdateRepuestoButton.Visibility = Visibility.Collapsed;
            _repuestoToEdit = null;
            LimpiarCamposRepuesto();
        }

        private void DeleteRepuestoButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRepuesto = RepuestosDataGrid.SelectedItem as Repuesto;
            if (selectedRepuesto == null)
            {
                MessageBox.Show("Por favor, selecciona un repuesto para eliminar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Repuestos.Remove(selectedRepuesto);
        }

        private bool ValidateRepuestoInputs(out int cantidad, out decimal precio, out TipoRepuesto? selectedTipoRepuesto)
        {
            cantidad = 0;
            precio = 0;
            selectedTipoRepuesto = null;

            if (string.IsNullOrWhiteSpace(RepuestoTextBox.Text))
            {
                MessageBox.Show("Debe ingresar un nombre para el repuesto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(CantidadTextBox.Text) || !int.TryParse(CantidadTextBox.Text, out cantidad) || cantidad <= 0)
            {
                MessageBox.Show("La cantidad debe ser un número entero positivo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(PrecioTextBox.Text) || !decimal.TryParse(PrecioTextBox.Text, out precio) || precio < 0)
            {
                MessageBox.Show("El precio debe ser un número válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            selectedTipoRepuesto = TipoRepuestoComboBox.SelectedItem as TipoRepuesto;
            if (selectedTipoRepuesto == null)
            {
                MessageBox.Show("Debe seleccionar un tipo de repuesto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
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
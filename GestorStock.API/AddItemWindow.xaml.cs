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

namespace GestorStock.API
{
    [SupportedOSPlatform("windows")]
    public partial class AddItemWindow : Window
    {
        private readonly ITipoExplotacionService _tipoExplotacionService;
        private readonly IRepuestoService _repuestoService;
        private readonly ITipoRepuestoService _tipoRepuestoService;
        private readonly ITipoItemService _tipoItemService;

        private ObservableCollection<Repuesto> _repuestos;
        private ObservableCollection<TipoExplotacion> _tipoExplotaciones;
        private ObservableCollection<TipoRepuesto> _tipoRepuestos;
        private ObservableCollection<TipoItem> _tipoItems;

        public Item NewItem { get; private set; }
        private Item? _itemToEdit;

        // Constructor para CREAR un nuevo ítem
        public AddItemWindow(ITipoExplotacionService tipoExplotacionService, IRepuestoService repuestoService, ITipoRepuestoService tipoRepuestoService, ITipoItemService tipoItemService)
        {
            InitializeComponent();
            _tipoExplotacionService = tipoExplotacionService;
            _repuestoService = repuestoService;
            _tipoRepuestoService = tipoRepuestoService;
            _tipoItemService = tipoItemService;

            _repuestos = new ObservableCollection<Repuesto>();
            RepuestosListView.ItemsSource = _repuestos;
            _tipoExplotaciones = new ObservableCollection<TipoExplotacion>();
            TipoExplotacionComboBox.ItemsSource = _tipoExplotaciones;
            _tipoRepuestos = new ObservableCollection<TipoRepuesto>();
            TipoRepuestoComboBox.ItemsSource = _tipoRepuestos;
            _tipoItems = new ObservableCollection<TipoItem>();
            TipoItemComboBox.ItemsSource = _tipoItems;

            NewItem = new Item();

            this.Loaded += AddItemWindow_Loaded;
            AddRepuestoButton.Click += AddRepuestoButton_Click;
            AcceptButton.Click += AcceptButton_Click;
            CancelButton.Click += CancelButton_Click;
            CantidadTextBox.PreviewTextInput += CantidadTextBox_PreviewTextInput;
        }

        // Constructor para EDITAR un ítem existente
        public AddItemWindow(ITipoExplotacionService tipoExplotacionService, IRepuestoService repuestoService, ITipoRepuestoService tipoRepuestoService, ITipoItemService tipoItemService, Item itemToEdit)
             : this(tipoExplotacionService, repuestoService, tipoRepuestoService, tipoItemService)
        {
            _itemToEdit = itemToEdit;
            this.Title = "Editar Ítem";
            LoadItemData();
        }

        private async void AddItemWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var tiposExplotacion = await _tipoExplotacionService.GetAllTipoExplotacionAsync();
                _tipoExplotaciones.Clear();
                foreach (var tipo in tiposExplotacion)
                {
                    _tipoExplotaciones.Add(tipo);
                }

                var tiposRepuesto = await _tipoRepuestoService.GetAllTiposAsync();
                _tipoRepuestos.Clear();
                foreach (var tipoRepuesto in tiposRepuesto)
                {
                    _tipoRepuestos.Add(tipoRepuesto);
                }

                var tiposItem = await _tipoItemService.GetAllTipoItemAsync();
                _tipoItems.Clear();
                foreach (var tipoItem in tiposItem)
                {
                    _tipoItems.Add(tipoItem);
                }

                if (_itemToEdit != null)
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
            if (_itemToEdit == null) return;
            NombreItemTextBox.Text = _itemToEdit.NombreItem;
            TipoExplotacionComboBox.SelectedItem = _tipoExplotaciones.FirstOrDefault(t => t.Id == _itemToEdit.TipoExplotacionId);
            TipoItemComboBox.SelectedItem = _tipoItems.FirstOrDefault(t => t.Id == _itemToEdit.TipoItemId);
            _repuestos.Clear();
            foreach (var repuesto in _itemToEdit.Repuestos)
            {
                _repuestos.Add(repuesto);
            }
        }

        private void AddRepuestoButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTipoRepuesto = TipoRepuestoComboBox.SelectedItem as TipoRepuesto;
            if (selectedTipoRepuesto == null)
            {
                MessageBox.Show("Debe seleccionar un tipo de repuesto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(RepuestoTextBox.Text) || string.IsNullOrWhiteSpace(CantidadTextBox.Text))
            {
                MessageBox.Show("Debe ingresar el nombre y la cantidad del repuesto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!int.TryParse(CantidadTextBox.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("La cantidad debe ser un número entero positivo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newRepuesto = new Repuesto
            {
                Nombre = RepuestoTextBox.Text,
                Cantidad = cantidad,
                TipoRepuesto = selectedTipoRepuesto
            };
            _repuestos.Add(newRepuesto);

            RepuestoTextBox.Clear();
            CantidadTextBox.Clear();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTipoExplotacion = TipoExplotacionComboBox.SelectedItem as TipoExplotacion;
            if (selectedTipoExplotacion == null)
            {
                MessageBox.Show("Debe seleccionar un tipo de explotación válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var selectedTipoItem = TipoItemComboBox.SelectedItem as TipoItem;
            if (selectedTipoItem == null)
            {
                MessageBox.Show("Debe seleccionar un tipo de ítem válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!_repuestos.Any())
            {
                MessageBox.Show("Debe agregar al menos un repuesto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_itemToEdit != null)
            {
                _itemToEdit.TipoExplotacion = selectedTipoExplotacion;
                _itemToEdit.TipoItem = selectedTipoItem;
                _itemToEdit.NombreItem = NombreItemTextBox.Text;
                _itemToEdit.Repuestos = _repuestos.ToList();
                NewItem = _itemToEdit;
            }
            else
            {
                NewItem.TipoExplotacion = selectedTipoExplotacion;
                NewItem.TipoItem = selectedTipoItem;
                NewItem.NombreItem = NombreItemTextBox.Text;
                NewItem.Repuestos = _repuestos.ToList();
            }

            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public Item GetItem()
        {
            return NewItem;
        }

        private void CantidadTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
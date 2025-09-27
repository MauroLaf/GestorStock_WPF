using GestorStock.Model.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace GestorStock.API
{
    public partial class EditRepuestoWindow : Window
    {
        public Repuesto EditedRepuesto { get; private set; }

        public EditRepuestoWindow(Repuesto repuestoToEdit, List<TipoRepuesto> tiposRepuesto)
        {
            InitializeComponent();

            // Crea una copia del objeto para editar, para no modificar el original directamente
            EditedRepuesto = new Repuesto
            {
                Id = repuestoToEdit.Id,
                Nombre = repuestoToEdit.Nombre,
                Cantidad = repuestoToEdit.Cantidad,
                TipoRepuesto = repuestoToEdit.TipoRepuesto,
                TipoRepuestoId = repuestoToEdit.TipoRepuestoId
            };

            this.DataContext = EditedRepuesto;
            TipoRepuestoComboBox.ItemsSource = tiposRepuesto;

            // Pre-selecciona el tipo de repuesto
            TipoRepuestoComboBox.SelectedItem = tiposRepuesto.FirstOrDefault(t => t.Id == EditedRepuesto.TipoRepuestoId);
        }

        private void AceptarButton_Click(object sender, RoutedEventArgs e)
        {
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
            if (TipoRepuestoComboBox.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un tipo de repuesto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            EditedRepuesto.Nombre = RepuestoTextBox.Text;
            EditedRepuesto.Cantidad = cantidad;
            EditedRepuesto.TipoRepuesto = (TipoRepuesto)TipoRepuestoComboBox.SelectedItem;
            EditedRepuesto.TipoRepuestoId = EditedRepuesto.TipoRepuesto.Id;

            this.DialogResult = true; // Esto es lo que hace que la línea `if (editWindow.ShowDialog() == true)` en AddItemWindow se ejecute.
        }

        private void CantidadTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
using GestorStock.Model.Entities;
using GestorStock.Services.Interfaces;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GestorStock.API
{
    public partial class DiscountRepuestoWindow : Window
    {
        private readonly IRepuestoService _repuestoService;
        private readonly List<Repuesto> _repuestos;

        public DiscountRepuestoWindow(IRepuestoService repuestoService, List<Repuesto> repuestos)
        {
            InitializeComponent();
            _repuestoService = repuestoService;
            _repuestos = repuestos;
            RepuestosListBox.ItemsSource = _repuestos;
            this.DataContext = this;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRepuesto = RepuestosListBox.SelectedItem as Repuesto;
            if (selectedRepuesto == null)
            {
                MessageBox.Show("Por favor, selecciona un repuesto para descontar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(CantidadTextBox.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Por favor, ingresa una cantidad válida.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await _repuestoService.DescontarUnidadAsync(selectedRepuesto.Id, cantidad);
            MessageBox.Show("Unidades descontadas exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CantidadTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Valida que solo se ingresen números
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }
    }
}

using System;
using System.Windows;
using GestorStock.Data;
using GestorStock.Services.Interfaces;

namespace GestorStock.API.Views
{
    public partial class MainWindow : Window
    {
        private readonly StockDbContext _ctx;
        private readonly IPedidoService _pedidos;
        private readonly IServiceProvider _sp; // para abrir otras ventanas con DI

        // INYECCIÓN POR CONSTRUCTOR
        public MainWindow(StockDbContext ctx, IPedidoService pedidos, IServiceProvider sp)
        {
            InitializeComponent();
            _ctx = ctx;
            _pedidos = pedidos;
            _sp = sp;

            // si usas code-behind, tu lógica puede quedarse aquí
            // ej.: cargar combos, etc.
            // Loaded += async (_,__) => { ... }
        }

        // Ejemplo de abrir otra ventana con DI por constructor
        private void BtnNuevaLinea_Click(object sender, RoutedEventArgs e)
        {
            var win = (AddItemWindow)_sp.GetService(typeof(AddItemWindow))!;
            win.Owner = this;
            win.ShowDialog();
        }
    }
}

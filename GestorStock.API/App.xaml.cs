using GestorStock.Data.Repositories;
using GestorStock.Services.Implementations;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System.Threading.Tasks;
using GestorStock.Model.Entities;

namespace GestorStock.API
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            // Establece la licencia no comercial para EPPlus
            ExcelPackage.License.SetNonCommercialPersonal("MAURO");

            IServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Configuración de servicios y DB Context
            services.AddDbContext<StockDbContext>();
            services.AddSingleton<ITipoItemService, TipoItemService>();
            services.AddSingleton<IPedidoService, PedidoService>();

            // CORRECCIÓN: Se utiliza la clase de implementación 'ItemService' 
            // en lugar de la interfaz 'IItemService' dos veces.
            services.AddSingleton<IItemService, ItemService>();

            services.AddSingleton<ITipoExplotacionService, TipoExplotacionService>();
            services.AddSingleton<IRepuestoService, RepuestoService>();
            services.AddSingleton<ITipoRepuestoService, TipoRepuestoService>();
            services.AddTransient<MainWindow>();
            services.AddTransient<CreatePedidoWindow>();
            services.AddTransient<AddItemWindow>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            // Lógica para verificar pedidos vencidos y mostrar alerta al inicio
            try
            {
                var pedidoService = _serviceProvider.GetRequiredService<IPedidoService>();

                // Obtenemos todos los pedidos de forma asíncrona
                var todosLosPedidos = await pedidoService.GetAllPedidosAsync();

                // Filtramos usando la propiedad calculada 'EstaVencido' de la clase Pedido.
                // Esta propiedad ya maneja los valores nulos (DateTime?) correctamente.
                var pedidosVencidos = todosLosPedidos.Where(p => p.EstaVencido).ToList();

                // Mostramos la alerta si hay pedidos vencidos
                if (pedidosVencidos.Any())
                {
                    var mensaje = $"¡ALERTA DE PEDIDOS VENCIDOS!\n\nSe han encontrado {pedidosVencidos.Count} pedido(s) cuya fecha de llegada ha pasado.";
                    MessageBox.Show(mensaje, "Pedidos Vencidos", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores de base de datos o servicios durante el arranque
                MessageBox.Show($"Error al verificar pedidos vencidos: {ex.Message}", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Continuamos con el arranque normal de la aplicación
            base.OnStartup(e);
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            if (mainWindow == null)
            {
                MessageBox.Show("Error al iniciar la aplicación. La ventana principal no se pudo inicializar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            mainWindow.Show();
        }
    }
}

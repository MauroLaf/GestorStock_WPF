using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using OfficeOpenXml;
using System.Threading.Tasks;
using GestorStock.Data; // Asegúrate de que el using sea GestorStock.Data
using GestorStock.Services.Interfaces;
using GestorStock.Services.Implementations;
using GestorStock.Model.Entities;

namespace GestorStock.API
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        private IConfiguration Configuration { get; } // Cambia a un get;

        public App()
        {
            ExcelPackage.License.SetNonCommercialPersonal("MAURO");

            IServiceCollection services = new ServiceCollection();
            // Construye la configuración aquí y la pasa al método de configuración
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);

            string connectionString = Configuration.GetConnectionString("DefaultConnection")?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no está configurada en appsettings.json.");

            services.AddDbContext<StockDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.Parse("8.0.21-mysql"))
            );

            // Aquí puedes agregar tus servicios y ventanas como lo tenías
            services.AddSingleton<ITipoItemService, TipoItemService>();
            services.AddSingleton<IPedidoService, PedidoService>();
            services.AddSingleton<IItemService, ItemService>();
            services.AddSingleton<ITipoExplotacionService, TipoExplotacionService>();
            services.AddSingleton<IRepuestoService, RepuestoService>();
            services.AddSingleton<ITipoRepuestoService, TipoRepuestoService>();

            // Las ventanas también se agregan aquí
            services.AddTransient<MainWindow>();
            services.AddTransient<CreatePedidoWindow>();
            services.AddTransient<AddItemWindow>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                var pedidoService = _serviceProvider.GetRequiredService<IPedidoService>();
                var todosLosPedidos = await pedidoService.GetAllPedidosAsync();
                var pedidosVencidos = todosLosPedidos.Where(p => p.EstaVencido).ToList();

                if (pedidosVencidos.Any())
                {
                    var mensaje = $"¡ALERTA DE PEDIDOS VENCIDOS!\n\nSe han encontrado {pedidosVencidos.Count} pedido(s) cuya fecha de llegada ha pasado.";
                    MessageBox.Show(mensaje, "Pedidos Vencidos", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al verificar pedidos vencidos: {ex.Message}", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            base.OnStartup(e);

            // Usa GetRequiredService<T>() en su lugar para garantizar que el servicio exista.
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
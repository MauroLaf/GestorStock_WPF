using GestorStock.Data.Repositories;
using GestorStock.Services.Implementations;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace GestorStock.API
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            ExcelPackage.License.SetNonCommercialPersonal("MAURO");

            IServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<StockDbContext>();

            services.AddSingleton<ITipoItemService, TipoItemService>();
            services.AddSingleton<IPedidoService, PedidoService>();
            services.AddSingleton<IItemService, ItemService>();
            services.AddSingleton<ITipoExplotacionService, TipoExplotacionService>();
            services.AddSingleton<IRepuestoService, RepuestoService>();
            services.AddSingleton<ITipoRepuestoService, TipoRepuestoService>();

            services.AddTransient<MainWindow>();
            services.AddTransient<CreatePedidoWindow>();
            services.AddTransient<AddItemWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
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

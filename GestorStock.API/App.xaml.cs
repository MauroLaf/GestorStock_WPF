using System;
using System.Windows;
using GestorStock.Data;
using GestorStock.Services.Implementations;
using GestorStock.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestorStock.API
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = default!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var cfg = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var cs = cfg.GetConnectionString("Default")
                  ?? "server=localhost;port=3306;database=GestorStockDb;user=gestor;password=12345;";

            var sc = new ServiceCollection();

            sc.AddDbContext<StockDbContext>(o => o.UseMySql(cs, ServerVersion.AutoDetect(cs)));

            // Servicios
            sc.AddScoped<IFamiliaService, FamiliaService>();
            sc.AddScoped<IUbicacionProductoService, UbicacionProductoService>();
            sc.AddScoped<IProveedorService, ProveedorService>();
            sc.AddScoped<ITipoSoporteService, TipoSoporteService>();
            sc.AddScoped<ITipoRepuestoService, TipoRepuestoService>();
            sc.AddScoped<IPedidoService, PedidoService>();
            sc.AddScoped<IRepuestoService, RepuestoService>();

            // Ventanas
            sc.AddTransient<Views.MainWindow>();
            sc.AddTransient<Views.CreatePedidoWindow>();
            sc.AddTransient<Views.AddItemWindow>();

            Services = sc.BuildServiceProvider();

            var main = Services.GetRequiredService<Views.MainWindow>();
            main.Show();
        }
    }
}

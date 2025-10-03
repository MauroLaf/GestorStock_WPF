using System;
using System.Windows;
using GestorStock.Data;
using GestorStock.Services.Interfaces;
using GestorStock.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestorStock.API
{
    public partial class App : Application
    {
        // Acceso global al contenedor (para tus code-behind)
        public static IServiceProvider Services { get; private set; } = default!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1) Config
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var cs = config.GetConnectionString("Default")
                     ?? "server=localhost;port=3306;database=GestorStockDb;user=gestor;password=12345;";

            // 2) DI
            var sc = new ServiceCollection();

            // DbContext (MySQL con Pomelo)
            sc.AddDbContext<StockDbContext>(o => o.UseMySql(cs, ServerVersion.AutoDetect(cs)));

            // Servicios (CRUDs)
            sc.AddScoped<IFamiliaService, FamiliaService>();
            sc.AddScoped<IUbicacionProductoService, UbicacionProductoService>();
            sc.AddScoped<IProveedorService, ProveedorService>();
            sc.AddScoped<ITipoSoporteService, TipoSoporteService>();
            sc.AddScoped<ITipoRepuestoService, TipoRepuestoService>(); // enum (no tabla)
            sc.AddScoped<IPedidoService, PedidoService>();
            sc.AddScoped<IRepuestoService, RepuestoService>();

            // Ventanas (registra las que uses)
            sc.AddTransient<MainWindow>();            // tu ventana principal existente
            sc.AddTransient<CreatePedidoWindow>();    // si la usas
            sc.AddTransient<AddItemWindow>();         // si la usas

            Services = sc.BuildServiceProvider();

            // 3) Abrir tu ventana principal actual (manteniendo code-behind)
            var main = Services.GetRequiredService<MainWindow>();
            main.Show();
        }
    }
}

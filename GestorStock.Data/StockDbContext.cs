using GestorStock.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestorStock.Data
{
    public class StockDbContext : DbContext
    {
        public StockDbContext(DbContextOptions<StockDbContext> options) : base(options) { }

        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Repuesto> Repuestos { get; set; }
        public DbSet<TipoRepuesto> TipoRepuestos { get; set; }
        public DbSet<Familia> Familias { get; set; }
        public DbSet<TipoSoporte> TiposSoporte { get; set; }
        public DbSet<UbicacionProducto> UbicacionProductos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Repuesto>()
                .Property(r => r.Precio)
                .HasColumnType("decimal(11, 2)");

            // Definición de las relaciones
            modelBuilder.Entity<Pedido>().HasMany(p => p.Items).WithOne(i => i.Pedido).HasForeignKey(i => i.PedidoId);
            modelBuilder.Entity<Item>().HasMany(i => i.Repuestos).WithOne(r => r.Item).HasForeignKey(r => r.ItemId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Repuesto>().HasOne(r => r.TipoRepuesto).WithMany(tr => tr.Repuestos).HasForeignKey(r => r.TipoRepuestoId);

            // CORRECCIÓN: Se cambió "TipoItemId" por "TipoSoporteId" para que coincida con la clase Item.
            modelBuilder.Entity<Item>().HasOne(i => i.TipoSoporte).WithMany(ts => ts.Items).HasForeignKey(i => i.TipoSoporteId);

            modelBuilder.Entity<Item>().HasOne(i => i.UbicacionProducto).WithMany(up => up.Items).HasForeignKey(i => i.UbicacionProductoId);
            modelBuilder.Entity<UbicacionProducto>().HasOne(up => up.Familia).WithMany(f => f.UbicacionProductos).HasForeignKey(up => up.FamiliaId);

            // Datos Iniciales (Seeding)
            modelBuilder.Entity<TipoRepuesto>().HasData(
                new TipoRepuesto { Id = 1, Nombre = "Original" },
                new TipoRepuesto { Id = 2, Nombre = "Usado" }
            );
            modelBuilder.Entity<Familia>().HasData(
                new Familia { Id = 1, Nombre = "INTERCAMBIADORES" },
                new Familia { Id = 2, Nombre = "MERCADOS" },
                new Familia { Id = 3, Nombre = "SETAS DE SEVILLA" },
                new Familia { Id = 4, Nombre = "EXPLOTACIONES" }
            );
            modelBuilder.Entity<TipoSoporte>().HasData(
                new TipoSoporte { Id = 1, Nombre = "Pantalla LED" },
                new TipoSoporte { Id = 2, Nombre = "Mupis Digital" },
                new TipoSoporte { Id = 3, Nombre = "Monoposte" },
                new TipoSoporte { Id = 4, Nombre = "Skyled" }
            );

            // Ubicaciones de INTERCAMBIADORES (FamiliaId = 1)
            modelBuilder.Entity<UbicacionProducto>().HasData(
                new UbicacionProducto { Id = 5, Nombre = "Skyled moncloa", FamiliaId = 1 },
                new UbicacionProducto { Id = 6, Nombre = "Skyled plaza elíptica fachada", FamiliaId = 1 },
                new UbicacionProducto { Id = 7, Nombre = "Skyled plaza elíptica lateral a-42", FamiliaId = 1 },
                new UbicacionProducto { Id = 15, Nombre = "Skyled estación autobuses donostia pantalla 4x1 entrada", FamiliaId = 1 },
                new UbicacionProducto { Id = 16, Nombre = "Skyled estación autobuses donostia cortina digital 2x2 lateral", FamiliaId = 1 },
                new UbicacionProducto { Id = 17, Nombre = "Pantalla led exterior pasaje acceso estación autobuses donostia", FamiliaId = 1 },
                new UbicacionProducto { Id = 18, Nombre = "Pantalla led pasillo acceso estación autobuses donostia", FamiliaId = 1 },
                new UbicacionProducto { Id = 19, Nombre = "Pantalla led 2x1.50 friso escalera acceso estación autobuses donostia", FamiliaId = 1 },
                new UbicacionProducto { Id = 20, Nombre = "Pantalla lcd 55\" hall estación autobuses donostia junto restaurante", FamiliaId = 1 },
                new UbicacionProducto { Id = 21, Nombre = "Pantalla led gran formato (bilbao)", FamiliaId = 1 },
                new UbicacionProducto { Id = 22, Nombre = "Circuito handia bilbao (bilbao)", FamiliaId = 1 },
                new UbicacionProducto { Id = 23, Nombre = "Circuito kaixo bilbao (bilbao)", FamiliaId = 1 },
                new UbicacionProducto { Id = 24, Nombre = "Columna digital conexión metro a islas 2-3 planta 3 (moncloa)", FamiliaId = 1 },
                new UbicacionProducto { Id = 25, Nombre = "Pantalla digital acceso metro isla 2 (moncloa)", FamiliaId = 1 },
                new UbicacionProducto { Id = 26, Nombre = "Pantalla digital acceso metro isla 3 (moncloa)", FamiliaId = 1 },
                new UbicacionProducto { Id = 27, Nombre = "Pantalla digital acceso p° moret (moncloa)", FamiliaId = 1 },
                new UbicacionProducto { Id = 28, Nombre = "40 mupis digitales (moncloa)", FamiliaId = 1 },
                new UbicacionProducto { Id = 29, Nombre = "20 mupis digitales (plaza elíptica)", FamiliaId = 1 },
                new UbicacionProducto { Id = 30, Nombre = "Pantalla digital nivel -1 (plaza elíptica)", FamiliaId = 1 },

                // Ubicaciones de MERCADOS (FamiliaId = 2)
                new UbicacionProducto { Id = 31, Nombre = "Mercado plaza de abastos", FamiliaId = 2 },
                new UbicacionProducto { Id = 32, Nombre = "Mercado de chamartín", FamiliaId = 2 },
                new UbicacionProducto { Id = 33, Nombre = "Mercado de correos", FamiliaId = 2 },
                new UbicacionProducto { Id = 34, Nombre = "Mercado central zaragoza", FamiliaId = 2 },
                new UbicacionProducto { Id = 35, Nombre = "Mercado de el este", FamiliaId = 2 },
                new UbicacionProducto { Id = 36, Nombre = "Mercado de la imprenta valencia", FamiliaId = 2 },
                new UbicacionProducto { Id = 37, Nombre = "Mercado de la carne", FamiliaId = 2 },
                new UbicacionProducto { Id = 38, Nombre = "Mercado los mostenses", FamiliaId = 2 },
                new UbicacionProducto { Id = 39, Nombre = "Mercado vell", FamiliaId = 2 },
                new UbicacionProducto { Id = 40, Nombre = "Mercado de la ribera", FamiliaId = 2 },
                new UbicacionProducto { Id = 41, Nombre = "Mercado de san antón", FamiliaId = 2 },
                new UbicacionProducto { Id = 42, Nombre = "Mercado de san antón parking", FamiliaId = 2 },
                new UbicacionProducto { Id = 43, Nombre = "Mercado san fernando", FamiliaId = 2 },
                new UbicacionProducto { Id = 44, Nombre = "Mercado de san ildefonso", FamiliaId = 2 },
                new UbicacionProducto { Id = 45, Nombre = "Mercado de san miguel", FamiliaId = 2 },
                new UbicacionProducto { Id = 46, Nombre = "Mercado de triana", FamiliaId = 2 },
                new UbicacionProducto { Id = 47, Nombre = "Mercado del val", FamiliaId = 2 },
                new UbicacionProducto { Id = 48, Nombre = "Cúpula del milenio (valladolid)", FamiliaId = 2 },

                // Ubicaciones de SETAS DE SEVILLA (FamiliaId = 3)
                new UbicacionProducto { Id = 10, Nombre = "Skyled las setas de sevilla columna", FamiliaId = 3 },
                new UbicacionProducto { Id = 11, Nombre = "Skyled las setas de sevilla mupi", FamiliaId = 3 },
                new UbicacionProducto { Id = 12, Nombre = "Skyled las setas de sevilla rampa", FamiliaId = 3 },
                new UbicacionProducto { Id = 13, Nombre = "Skyled las setas de sevilla mercado", FamiliaId = 3 },
                new UbicacionProducto { Id = 14, Nombre = "Skyled las setas de sevilla parasol", FamiliaId = 3 },

                // Ubicaciones de EXPLOTACIONES (FamiliaId = 4)
                new UbicacionProducto { Id = 1, Nombre = "Skyled basauri (cc bilbondo)", FamiliaId = 4 },
                new UbicacionProducto { Id = 2, Nombre = "Skyled cc niessen", FamiliaId = 4 },
                new UbicacionProducto { Id = 3, Nombre = "Skyled cc zubiarte (cc zubiarte)", FamiliaId = 4 },
                new UbicacionProducto { Id = 4, Nombre = "Skyled granada (cc granaita)", FamiliaId = 4 },
                new UbicacionProducto { Id = 8, Nombre = "Skyled valencia", FamiliaId = 4 },
                new UbicacionProducto { Id = 9, Nombre = "Skyled valladolid (cc vallsur)", FamiliaId = 4 }
            );
        }
    }
}
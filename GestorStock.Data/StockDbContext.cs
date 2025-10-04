using Microsoft.EntityFrameworkCore;
using GestorStock.Model.Entities;
using GestorStock.Model.Enum;

namespace GestorStock.Data
{
    public class StockDbContext : DbContext
    {
        public StockDbContext(DbContextOptions<StockDbContext> options) : base(options) { }

        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public DbSet<Repuesto> Repuestos => Set<Repuesto>();
        public DbSet<Familia> Familias => Set<Familia>();
        public DbSet<Proveedor> Proveedores => Set<Proveedor>();
        public DbSet<UbicacionProducto> UbicacionProductos => Set<UbicacionProducto>();
        public DbSet<TipoSoporte> TipoSoportes => Set<TipoSoporte>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Pedido =====
            modelBuilder.Entity<Pedido>(e =>
            {
                e.HasKey(p => p.Id);

                e.HasOne(p => p.Familia)
                 .WithMany(f => f.Pedidos)       // 1 Familia -> N Pedidos
                 .HasForeignKey(p => p.FamiliaId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.Restrict); // impide borrar una Familia con pedidos

                e.HasMany(p => p.Repuestos)
                 .WithOne(r => r.Pedido)
                 .HasForeignKey(r => r.PedidoId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.Cascade);  // borrar Pedido borra sus Repuestos
            });

            // ===== Repuesto =====
            modelBuilder.Entity<Repuesto>(e =>
            {
                e.HasKey(r => r.Id);

                // (Opcionales de calidad)
                e.Property(r => r.Nombre).IsRequired();
                e.Property(r => r.Precio).HasColumnType("decimal(18,2)");

                e.HasOne(r => r.Familia)
                 .WithMany(f => f.Repuestos)
                 .HasForeignKey(r => r.FamiliaId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(r => r.UbicacionProducto)
                 .WithMany(u => u.Repuestos)
                 .HasForeignKey(r => r.UbicacionProductoId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(r => r.Proveedor)
                 .WithMany(pv => pv.Repuestos)
                 .HasForeignKey(r => r.ProveedorId)
                 .OnDelete(DeleteBehavior.Restrict);

                // TipoSoporte OBLIGATORIO por línea (como pediste)
                e.HasOne(r => r.TipoSoporte)
                 .WithMany(ts => ts.Repuestos)
                 .HasForeignKey(r => r.TipoSoporteId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Proveedor>().HasData(
                new Proveedor { ProveedorId = 1, Nombre = "Proveedor Prueba" }
            );

            modelBuilder.Entity<Repuesto>().HasData(
                new Repuesto { Id = 1, Nombre = "Repuesto Prueba" }
            );

            // ================== SEEDING ==================
            // 1) Proveedor demo
            modelBuilder.Entity<Proveedor>().HasData(
                new Proveedor { ProveedorId = 1, Nombre = "Proveedor Demo" }
            );

            // 2) Pedido demo (usa una fecha fija para evitar difs futuras de migración)
            modelBuilder.Entity<Pedido>().HasData(
                new Pedido { Id = 1, FechaCreacion = new DateTime(2025, 10, 04), FamiliaId = 1, Descripcion = "Pedido demo" }
            );

            // 3) Repuesto demo con FKs válidas y coherentes (UbicacionProductoId=5 es de FamiliaId=1)
            modelBuilder.Entity<Repuesto>().HasData(
                new Repuesto
                {
                    Id = 1,
                    Nombre = "Repuesto Demo",
                    Descripcion = "",
                    Cantidad = 1,
                    Precio = 0m,
                    TipoRepuesto = TipoRepuestoEnum.Original,          // el enum que uses
                    FamiliaId = 1,
                    UbicacionProductoId = 5,
                    ProveedorId = 1,
                    TipoSoporteId = 1,
                    PedidoId = 1
                }
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

            modelBuilder.Entity<UbicacionProducto>().HasData(
                // INTERCAMBIADORES (FamiliaId = 1)
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

                // MERCADOS (FamiliaId = 2)
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

                // SETAS DE SEVILLA (FamiliaId = 3)
                new UbicacionProducto { Id = 10, Nombre = "Skyled las setas de sevilla columna", FamiliaId = 3 },
                new UbicacionProducto { Id = 11, Nombre = "Skyled las setas de sevilla mupi", FamiliaId = 3 },
                new UbicacionProducto { Id = 12, Nombre = "Skyled las setas de sevilla rampa", FamiliaId = 3 },
                new UbicacionProducto { Id = 13, Nombre = "Skyled las setas de sevilla mercado", FamiliaId = 3 },
                new UbicacionProducto { Id = 14, Nombre = "Skyled las setas de sevilla parasol", FamiliaId = 3 },

                // EXPLOTACIONES (FamiliaId = 4)
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

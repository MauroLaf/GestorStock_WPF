using GestorStock.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestorStock.Data.Repositories
{
    public class StockDbContext : DbContext
    {
        public StockDbContext(DbContextOptions<StockDbContext> options) : base(options) { }
        public StockDbContext() { }

        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Repuesto> Repuestos { get; set; }
        public DbSet<Explotacion> Explotaciones { get; set; }
        public DbSet<TipoRepuesto> TipoRepuestos { get; set; }
        public DbSet<TipoExplotacion> TipoExplotaciones { get; set; }
        public DbSet<TipoSoporte> TiposItem { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                const string connectionString = "Server=localhost;Port=3306;Database=GestorStockDb;Uid=gestor;Pwd=12345;";
                optionsBuilder.UseMySql(connectionString, ServerVersion.Parse("8.0.21-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // CONFIGURACIÓN DE PRECISIÓN DECIMAL (11, 2)
            // Esto se aplica correctamente a la propiedad 'Precio' en la entidad 'Repuesto', 
            // asegurando 11 dígitos en total, con 2 decimales para la base de datos.
            modelBuilder.Entity<Repuesto>()
                .Property(r => r.Precio)
                .HasColumnType("decimal(11, 2)");


            // Relaciones
            modelBuilder.Entity<Pedido>().HasMany(p => p.Items).WithOne(i => i.Pedido).HasForeignKey(i => i.PedidoId);
            modelBuilder.Entity<Item>().HasMany(i => i.Repuestos).WithOne(r => r.Item).HasForeignKey(r => r.ItemId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Item>().HasOne(i => i.TipoExplotacion).WithMany(te => te.Items).HasForeignKey(i => i.TipoExplotacionId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Repuesto>().HasOne(r => r.TipoRepuesto).WithMany(tr => tr.Repuestos).HasForeignKey(r => r.TipoRepuestoId);
            modelBuilder.Entity<Item>().HasOne(i => i.TipoSoporte).WithMany(ti => ti.Items).HasForeignKey(i => i.TipoItemId);

            // Datos iniciales
            modelBuilder.Entity<TipoRepuesto>().HasData(
                new TipoRepuesto { Id = 1, Nombre = "Original" },
                new TipoRepuesto { Id = 2, Nombre = "Usado" }
            );
            modelBuilder.Entity<TipoExplotacion>().HasData(
                new TipoExplotacion { Id = 1, Nombre = "INTERCAMBIADOR" },
                new TipoExplotacion { Id = 2, Nombre = "MERCADO" },
                new TipoExplotacion { Id = 3, Nombre = "MONOPOSTE" },
                new TipoExplotacion { Id = 4, Nombre = "SKYLED" }
            );
            modelBuilder.Entity<TipoSoporte>().HasData(
                new TipoSoporte { Id = 1, Nombre = "Pantalla" },
                new TipoSoporte { Id = 2, Nombre = "Mupis" },
                new TipoSoporte { Id = 3, Nombre = "Monoposte" },
                new TipoSoporte { Id = 4, Nombre = "Skyled" }
            );
        }
    }
}

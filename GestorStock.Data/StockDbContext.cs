using GestorStock.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GestorStock.Data.Repositories
{
    public class StockDbContext : DbContext
    {
        // Constructor para la inyección de dependencias
        public StockDbContext(DbContextOptions<StockDbContext> options)
            : base(options)
        {
        }

        // Constructor para las herramientas de migración (no requiere parámetros)
        public StockDbContext()
        {
        }

        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Repuesto> Repuestos { get; set; }
        public DbSet<Explotacion> Explotaciones { get; set; }
        public DbSet<TipoRepuesto> TipoRepuestos { get; set; }
        public DbSet<TipoExplotacion> TipoExplotaciones { get; set; }
        public DbSet<TipoItem> TiposItem { get; set; }

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

            // Relación uno a muchos entre Pedido e Item
            modelBuilder.Entity<Pedido>()
                .HasMany(p => p.Items)
                .WithOne(i => i.Pedido)
                .HasForeignKey(i => i.PedidoId);

            // Relación uno a muchos entre Item y Repuesto
            modelBuilder.Entity<Item>()
                .HasMany(i => i.Repuestos)
                .WithOne(r => r.Item)
                .HasForeignKey(r => r.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación uno a muchos entre TipoExplotacion e Item
            modelBuilder.Entity<TipoExplotacion>()
                .HasMany(te => te.Items)
                .WithOne(i => i.TipoExplotacion)
                .HasForeignKey(i => i.TipoExplotacionId)
                .OnDelete(DeleteBehavior.Restrict);

            // *** ¡CORRECCIÓN CRÍTICA AQUÍ! ***
            // Relación uno a muchos entre TipoRepuesto y Repuesto
            // Forzamos a Entity Framework a usar la columna 'Tipo' en lugar de la convención TipoRepuestoId
            modelBuilder.Entity<Repuesto>()
                .HasOne(r => r.TipoRepuesto)
                .WithMany(tr => tr.Repuestos)
                .HasForeignKey("Tipo") // <-- ¡La clave foránea se llama 'Tipo'!
                .IsRequired(); // <-- ¡Aseguramos que el campo sea requerido!

            // Relación uno a muchos entre TipoItem e Item
            modelBuilder.Entity<Item>()
                .HasOne(i => i.TipoItem)
                .WithMany(ti => ti.Items)
                .HasForeignKey(i => i.TipoItemId)
                .IsRequired();

            // Inicializar datos para TipoRepuesto, TipoExplotacion y TipoItem
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

            modelBuilder.Entity<TipoItem>().HasData(
                new TipoItem { Id = 1, Nombre = "Pantalla" },
                new TipoItem { Id = 2, Nombre = "Mupis" },
                new TipoItem { Id = 3, Nombre = "Monoposte" },
                new TipoItem { Id = 4, Nombre = "Skyled" }
            );
        }
    }
}
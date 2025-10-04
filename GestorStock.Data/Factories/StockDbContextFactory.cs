using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GestorStock.Data.Factories
{
    public class StockDbContextFactory : IDesignTimeDbContextFactory<StockDbContext>
    {
        public StockDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<StockDbContext>();

            // Ajusta tu cadena de conexión
            var connectionString =
                //"server=localhost;port=3306;database=GestorStockDb;user=gestor;password=12345;";

            //conexion test
            "server=localhost;port=3306;database=GestorStockDbTest;user=gestor;password=12345;";


            // Pomelo.EntityFrameworkCore.MySql
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new StockDbContext(optionsBuilder.Options);
        }
    }
}

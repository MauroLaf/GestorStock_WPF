using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GestorStock.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigracionInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Familias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Familias", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Descripcion = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Incidencia = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaIncidencia = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DescripcionIncidencia = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaLlegada = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TipoRepuestos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoRepuestos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TiposSoporte",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposSoporte", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UbicacionProductos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FamiliaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UbicacionProductos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UbicacionProductos_Familias_FamiliaId",
                        column: x => x.FamiliaId,
                        principalTable: "Familias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    FamiliaId = table.Column<int>(type: "int", nullable: true),
                    UbicacionProductoId = table.Column<int>(type: "int", nullable: true),
                    TipoSoporteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Familias_FamiliaId",
                        column: x => x.FamiliaId,
                        principalTable: "Familias",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Items_Pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Items_TiposSoporte_TipoSoporteId",
                        column: x => x.TipoSoporteId,
                        principalTable: "TiposSoporte",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Items_UbicacionProductos_UbicacionProductoId",
                        column: x => x.UbicacionProductoId,
                        principalTable: "UbicacionProductos",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Repuestos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Precio = table.Column<decimal>(type: "decimal(11,2)", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: true),
                    TipoRepuestoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repuestos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Repuestos_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Repuestos_TipoRepuestos_TipoRepuestoId",
                        column: x => x.TipoRepuestoId,
                        principalTable: "TipoRepuestos",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Familias",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "INTERCAMBIADORES" },
                    { 2, "MERCADOS" },
                    { 3, "SETAS DE SEVILLA" },
                    { 4, "EXPLOTACIONES" }
                });

            migrationBuilder.InsertData(
                table: "TipoRepuestos",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Original" },
                    { 2, "Usado" }
                });

            migrationBuilder.InsertData(
                table: "TiposSoporte",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Pantalla LED" },
                    { 2, "Mupis Digital" },
                    { 3, "Monoposte" },
                    { 4, "Skyled" }
                });

            migrationBuilder.InsertData(
                table: "UbicacionProductos",
                columns: new[] { "Id", "FamiliaId", "Nombre" },
                values: new object[,]
                {
                    { 1, 4, "Skyled basauri (cc bilbondo)" },
                    { 2, 4, "Skyled cc niessen" },
                    { 3, 4, "Skyled cc zubiarte (cc zubiarte)" },
                    { 4, 4, "Skyled granada (cc granaita)" },
                    { 5, 1, "Skyled moncloa" },
                    { 6, 1, "Skyled plaza elíptica fachada" },
                    { 7, 1, "Skyled plaza elíptica lateral a-42" },
                    { 8, 4, "Skyled valencia" },
                    { 9, 4, "Skyled valladolid (cc vallsur)" },
                    { 10, 3, "Skyled las setas de sevilla columna" },
                    { 11, 3, "Skyled las setas de sevilla mupi" },
                    { 12, 3, "Skyled las setas de sevilla rampa" },
                    { 13, 3, "Skyled las setas de sevilla mercado" },
                    { 14, 3, "Skyled las setas de sevilla parasol" },
                    { 15, 1, "Skyled estación autobuses donostia pantalla 4x1 entrada" },
                    { 16, 1, "Skyled estación autobuses donostia cortina digital 2x2 lateral" },
                    { 17, 1, "Pantalla led exterior pasaje acceso estación autobuses donostia" },
                    { 18, 1, "Pantalla led pasillo acceso estación autobuses donostia" },
                    { 19, 1, "Pantalla led 2x1.50 friso escalera acceso estación autobuses donostia" },
                    { 20, 1, "Pantalla lcd 55\" hall estación autobuses donostia junto restaurante" },
                    { 21, 1, "Pantalla led gran formato (bilbao)" },
                    { 22, 1, "Circuito handia bilbao (bilbao)" },
                    { 23, 1, "Circuito kaixo bilbao (bilbao)" },
                    { 24, 1, "Columna digital conexión metro a islas 2-3 planta 3 (moncloa)" },
                    { 25, 1, "Pantalla digital acceso metro isla 2 (moncloa)" },
                    { 26, 1, "Pantalla digital acceso metro isla 3 (moncloa)" },
                    { 27, 1, "Pantalla digital acceso p° moret (moncloa)" },
                    { 28, 1, "40 mupis digitales (moncloa)" },
                    { 29, 1, "20 mupis digitales (plaza elíptica)" },
                    { 30, 1, "Pantalla digital nivel -1 (plaza elíptica)" },
                    { 31, 2, "Mercado plaza de abastos" },
                    { 32, 2, "Mercado de chamartín" },
                    { 33, 2, "Mercado de correos" },
                    { 34, 2, "Mercado central zaragoza" },
                    { 35, 2, "Mercado de el este" },
                    { 36, 2, "Mercado de la imprenta valencia" },
                    { 37, 2, "Mercado de la carne" },
                    { 38, 2, "Mercado los mostenses" },
                    { 39, 2, "Mercado vell" },
                    { 40, 2, "Mercado de la ribera" },
                    { 41, 2, "Mercado de san antón" },
                    { 42, 2, "Mercado de san antón parking" },
                    { 43, 2, "Mercado san fernando" },
                    { 44, 2, "Mercado de san ildefonso" },
                    { 45, 2, "Mercado de san miguel" },
                    { 46, 2, "Mercado de triana" },
                    { 47, 2, "Mercado del val" },
                    { 48, 2, "Cúpula del milenio (valladolid)" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_FamiliaId",
                table: "Items",
                column: "FamiliaId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_PedidoId",
                table: "Items",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_TipoSoporteId",
                table: "Items",
                column: "TipoSoporteId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_UbicacionProductoId",
                table: "Items",
                column: "UbicacionProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Repuestos_ItemId",
                table: "Repuestos",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Repuestos_TipoRepuestoId",
                table: "Repuestos",
                column: "TipoRepuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_UbicacionProductos_FamiliaId",
                table: "UbicacionProductos",
                column: "FamiliaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Repuestos");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "TipoRepuestos");

            migrationBuilder.DropTable(
                name: "Pedidos");

            migrationBuilder.DropTable(
                name: "TiposSoporte");

            migrationBuilder.DropTable(
                name: "UbicacionProductos");

            migrationBuilder.DropTable(
                name: "Familias");
        }
    }
}

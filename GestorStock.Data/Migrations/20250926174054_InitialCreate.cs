using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GestorStock.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Fecha = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Descripcion = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Incidencia = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaIncidencia = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DescripcionIncidencia = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TipoExplotaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoExplotaciones", x => x.Id);
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
                name: "TiposItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposItem", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Explotaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoExplotacionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Explotaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Explotaciones_TipoExplotaciones_TipoExplotacionId",
                        column: x => x.TipoExplotacionId,
                        principalTable: "TipoExplotaciones",
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
                    Tipo = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    NombreItem = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoExplotacionId = table.Column<int>(type: "int", nullable: false),
                    TipoItemId = table.Column<int>(type: "int", nullable: false),
                    ExplotacionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Explotaciones_ExplotacionId",
                        column: x => x.ExplotacionId,
                        principalTable: "Explotaciones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Items_Pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Items_TipoExplotaciones_TipoExplotacionId",
                        column: x => x.TipoExplotacionId,
                        principalTable: "TipoExplotaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_TiposItem_TipoItemId",
                        column: x => x.TipoItemId,
                        principalTable: "TiposItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    TipoRepuestoId = table.Column<int>(type: "int", nullable: false)
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "TipoExplotaciones",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "INTERCAMBIADOR" },
                    { 2, "MERCADO" },
                    { 3, "MONOPOSTE" },
                    { 4, "SKYLED" }
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
                table: "TiposItem",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Pantalla" },
                    { 2, "Mupis" },
                    { 3, "Monoposte" },
                    { 4, "Skyled" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Explotaciones_TipoExplotacionId",
                table: "Explotaciones",
                column: "TipoExplotacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ExplotacionId",
                table: "Items",
                column: "ExplotacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_PedidoId",
                table: "Items",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_TipoExplotacionId",
                table: "Items",
                column: "TipoExplotacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_TipoItemId",
                table: "Items",
                column: "TipoItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Repuestos_ItemId",
                table: "Repuestos",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Repuestos_TipoRepuestoId",
                table: "Repuestos",
                column: "TipoRepuestoId");
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
                name: "Explotaciones");

            migrationBuilder.DropTable(
                name: "Pedidos");

            migrationBuilder.DropTable(
                name: "TiposItem");

            migrationBuilder.DropTable(
                name: "TipoExplotaciones");
        }
    }
}

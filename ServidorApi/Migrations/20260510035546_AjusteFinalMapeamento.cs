using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServidorApi.Migrations
{
    /// <inheritdoc />
    public partial class AjusteFinalMapeamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_categoria",
                columns: table => new
                {
                    id_categoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome_categoria = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tipo_categoria = table.Column<string>(type: "varchar(7)", maxLength: 7, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_categoria", x => x.id_categoria);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_conta",
                columns: table => new
                {
                    id_conta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome_conta = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ativa_conta = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    id_usuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_conta", x => x.id_conta);
                    table.ForeignKey(
                        name: "FK_tb_conta_tb_usuario_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "tb_usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_entrada",
                columns: table => new
                {
                    id_entrada = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    descricao_entrada = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    valor_entrada = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    data_entrada = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    id_conta = table.Column<int>(type: "int", nullable: false),
                    id_categoria = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_entrada", x => x.id_entrada);
                    table.ForeignKey(
                        name: "FK_tb_entrada_tb_categoria_id_categoria",
                        column: x => x.id_categoria,
                        principalTable: "tb_categoria",
                        principalColumn: "id_categoria",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb_entrada_tb_conta_id_conta",
                        column: x => x.id_conta,
                        principalTable: "tb_conta",
                        principalColumn: "id_conta",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_saida",
                columns: table => new
                {
                    id_saida = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    valor_saida = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    data_saida = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    id_conta = table.Column<int>(type: "int", nullable: false),
                    id_categoria = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_saida", x => x.id_saida);
                    table.ForeignKey(
                        name: "FK_tb_saida_tb_categoria_id_categoria",
                        column: x => x.id_categoria,
                        principalTable: "tb_categoria",
                        principalColumn: "id_categoria",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb_saida_tb_conta_id_conta",
                        column: x => x.id_conta,
                        principalTable: "tb_conta",
                        principalColumn: "id_conta",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_saldo_conta",
                columns: table => new
                {
                    id_conta = table.Column<int>(type: "int", nullable: false),
                    total_entrada_conta = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    total_saida_conta = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    saldo_atual_conta = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ultima_atualizacao_conta = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_saldo_conta", x => x.id_conta);
                    table.ForeignKey(
                        name: "FK_tb_saldo_conta_tb_conta_id_conta",
                        column: x => x.id_conta,
                        principalTable: "tb_conta",
                        principalColumn: "id_conta",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_tb_conta_id_usuario",
                table: "tb_conta",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_tb_entrada_id_categoria",
                table: "tb_entrada",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_tb_entrada_id_conta",
                table: "tb_entrada",
                column: "id_conta");

            migrationBuilder.CreateIndex(
                name: "IX_tb_saida_id_categoria",
                table: "tb_saida",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_tb_saida_id_conta",
                table: "tb_saida",
                column: "id_conta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_entrada");

            migrationBuilder.DropTable(
                name: "tb_saida");

            migrationBuilder.DropTable(
                name: "tb_saldo_conta");

            migrationBuilder.DropTable(
                name: "tb_categoria");

            migrationBuilder.DropTable(
                name: "tb_conta");
        }
    }
}

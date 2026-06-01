using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServidorApi.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCampoAtivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ativo_usuario",
                table: "tb_usuario",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ativo_usuario",
                table: "tb_usuario");
        }
    }
}

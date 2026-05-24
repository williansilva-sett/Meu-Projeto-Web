using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServidorApi.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarTipoUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "tipo_usuario",
                table: "tb_usuario",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tipo_usuario",
                table: "tb_usuario");
        }
    }
}

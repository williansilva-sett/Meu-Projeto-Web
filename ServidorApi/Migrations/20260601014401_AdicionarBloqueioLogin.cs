using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServidorApi.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarBloqueioLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "bloqueado_ate",
                table: "tb_usuario",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "tentativas_login",
                table: "tb_usuario",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bloqueado_ate",
                table: "tb_usuario");

            migrationBuilder.DropColumn(
                name: "tentativas_login",
                table: "tb_usuario");
        }
    }
}

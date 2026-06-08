using System;
using Microsoft.EntityFrameworkCore.Migrations;
 
#nullable disable
 
namespace ServidorApi.Migrations
{
    /// <inheritdoc />
    public partial class VincularTransacoesAoUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Dropar FK da tb_conta para tb_usuario
            migrationBuilder.DropForeignKey(
                name: "FK_tb_conta_tb_usuario_id_usuario",
                table: "tb_conta");

            // 2. Dropar FKs de saida e entrada para tb_conta
            migrationBuilder.DropForeignKey(
                name: "FK_tb_saida_tb_conta_id_conta",
                table: "tb_saida");

            migrationBuilder.DropForeignKey(
                name: "FK_tb_entrada_tb_conta_id_conta",
                table: "tb_entrada");

 
            // 2. Dropar tb_saldo_conta
            migrationBuilder.DropTable(
                name: "tb_saldo_conta");

 
            // 3. Dropar índices antigos de saida e entrada
            migrationBuilder.DropIndex(
                name: "IX_tb_saida_id_conta",
                table: "tb_saida");
 
            migrationBuilder.DropIndex(
                name: "IX_tb_entrada_id_conta",
                table: "tb_entrada");
 
            // 4. Dropar índice de conta para recriar como único
            migrationBuilder.DropIndex(
                name: "IX_tb_conta_id_usuario",
                table: "tb_conta");
 
            // 5. Remover nome_conta da tb_conta
            migrationBuilder.DropColumn(
                name: "nome_conta",
                table: "tb_conta");
 
            // 6. Renomear ativa_conta para ativo_conta
            migrationBuilder.RenameColumn(
                name: "ativa_conta",
                table: "tb_conta",
                newName: "ativo_conta");
 
            // 7. Renomear id_conta para id_usuario em tb_saida e tb_entrada
            migrationBuilder.RenameColumn(
                name: "id_conta",
                table: "tb_saida",
                newName: "id_usuario");
 
            migrationBuilder.RenameColumn(
                name: "id_conta",
                table: "tb_entrada",
                newName: "id_usuario");
 
            // 8. Remover colunas de auth do tb_usuario
            migrationBuilder.DropColumn(
                name: "ativo_usuario",
                table: "tb_usuario");
 
            migrationBuilder.DropColumn(
                name: "bloqueado_ate",
                table: "tb_usuario");
 
            migrationBuilder.DropColumn(
                name: "dataCriacao_usuario",
                table: "tb_usuario");
 
            migrationBuilder.DropColumn(
                name: "eMail_usuario",
                table: "tb_usuario");
 
            migrationBuilder.DropColumn(
                name: "senha_usuario",
                table: "tb_usuario");
 
            migrationBuilder.DropColumn(
                name: "tentativas_login",
                table: "tb_usuario");
 
            migrationBuilder.DropColumn(
                name: "tipo_usuario",
                table: "tb_usuario");
 
            // 9. Adicionar colunas de auth na tb_conta
            migrationBuilder.AddColumn<DateTime>(
                name: "bloqueado_ate",
                table: "tb_conta",
                type: "datetime(6)",
                nullable: true);
 
            migrationBuilder.AddColumn<DateTime>(
                name: "data_criacao_conta",
                table: "tb_conta",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
 
            migrationBuilder.AddColumn<string>(
                name: "email_conta",
                table: "tb_conta",
                type: "varchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
 
            migrationBuilder.AddColumn<string>(
                name: "senha_conta",
                table: "tb_conta",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
 
            migrationBuilder.AddColumn<int>(
                name: "tentativas_login",
                table: "tb_conta",
                type: "int",
                nullable: false,
                defaultValue: 0);
 
            migrationBuilder.AddColumn<int>(
                name: "tipo_conta",
                table: "tb_conta",
                type: "int",
                nullable: false,
                defaultValue: 0);
 
            // 10. Recriar índices com novos nomes
            migrationBuilder.CreateIndex(
                name: "IX_tb_conta_id_usuario",
                table: "tb_conta",
                column: "id_usuario",
                unique: true);
 
            migrationBuilder.CreateIndex(
                name: "IX_tb_saida_id_usuario",
                table: "tb_saida",
                column: "id_usuario");
 
            migrationBuilder.CreateIndex(
                name: "IX_tb_entrada_id_usuario",
                table: "tb_entrada",
                column: "id_usuario");
 
            // 11. Adicionar novas FKs
            migrationBuilder.AddForeignKey(
                name: "FK_tb_entrada_tb_usuario_id_usuario",
                table: "tb_entrada",
                column: "id_usuario",
                principalTable: "tb_usuario",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);
 
            migrationBuilder.AddForeignKey(
                name: "FK_tb_saida_tb_usuario_id_usuario",
                table: "tb_saida",
                column: "id_usuario",
                principalTable: "tb_usuario",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);
        }
 
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_entrada_tb_usuario_id_usuario",
                table: "tb_entrada");
 
            migrationBuilder.DropForeignKey(
                name: "FK_tb_saida_tb_usuario_id_usuario",
                table: "tb_saida");
 
            migrationBuilder.DropIndex(
                name: "IX_tb_conta_id_usuario",
                table: "tb_conta");

            migrationBuilder.AddForeignKey(
                name: "FK_tb_conta_tb_usuario_id_usuario",
                table: "tb_conta",
                column: "id_usuario",
                principalTable: "tb_usuario",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);
 
            migrationBuilder.DropIndex(
                name: "IX_tb_saida_id_usuario",
                table: "tb_saida");
 
            migrationBuilder.DropIndex(
                name: "IX_tb_entrada_id_usuario",
                table: "tb_entrada");
 
            migrationBuilder.DropColumn(
                name: "bloqueado_ate",
                table: "tb_conta");
 
            migrationBuilder.DropColumn(
                name: "data_criacao_conta",
                table: "tb_conta");
 
            migrationBuilder.DropColumn(
                name: "email_conta",
                table: "tb_conta");
 
            migrationBuilder.DropColumn(
                name: "senha_conta",
                table: "tb_conta");
 
            migrationBuilder.DropColumn(
                name: "tentativas_login",
                table: "tb_conta");
 
            migrationBuilder.DropColumn(
                name: "tipo_conta",
                table: "tb_conta");
 
            migrationBuilder.RenameColumn(
                name: "ativo_conta",
                table: "tb_conta",
                newName: "ativa_conta");
 
            migrationBuilder.RenameColumn(
                name: "id_usuario",
                table: "tb_saida",
                newName: "id_conta");
 
            migrationBuilder.RenameColumn(
                name: "id_usuario",
                table: "tb_entrada",
                newName: "id_conta");
 
            migrationBuilder.AddColumn<string>(
                name: "nome_conta",
                table: "tb_conta",
                type: "varchar(45)",
                maxLength: 45,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
 
            migrationBuilder.CreateIndex(
                name: "IX_tb_conta_id_usuario",
                table: "tb_conta",
                column: "id_usuario");
 
            migrationBuilder.CreateIndex(
                name: "IX_tb_saida_id_conta",
                table: "tb_saida",
                column: "id_conta");
 
            migrationBuilder.CreateIndex(
                name: "IX_tb_entrada_id_conta",
                table: "tb_entrada",
                column: "id_conta");
 
            migrationBuilder.AddForeignKey(
                name: "FK_tb_entrada_tb_conta_id_conta",
                table: "tb_entrada",
                column: "id_conta",
                principalTable: "tb_conta",
                principalColumn: "id_conta",
                onDelete: ReferentialAction.Cascade);
 
            migrationBuilder.AddForeignKey(
                name: "FK_tb_saida_tb_conta_id_conta",
                table: "tb_saida",
                column: "id_conta",
                principalTable: "tb_conta",
                principalColumn: "id_conta",
                onDelete: ReferentialAction.Cascade);
 
            migrationBuilder.CreateTable(
                name: "tb_saldo_conta",
                columns: table => new
                {
                    id_conta = table.Column<int>(type: "int", nullable: false),
                    saldo_atual_conta = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    total_entrada_conta = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    total_saida_conta = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
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
        }
    }
}
using Microsoft.EntityFrameworkCore;
using ServidorApi.Models;

namespace ServidorApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        // Mapeamento da tabela de usuários
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Conta> Contas { get; set; }
        public DbSet<Saldo_Conta> SaldoContas { get; set; }
        public DbSet<Entradas> Entrada { get; set; }
        public DbSet<Saida> Saidas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
    
        // As próximas tabelas (Conta, Entrada, Saida) você adicionará aqui embaixo
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Isso força o mapeamento "no grito" se o atributo [Table] falhar
            modelBuilder.Entity<Entradas>().ToTable("tb_entrada");
        }
    }
}
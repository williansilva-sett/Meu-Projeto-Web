using Microsoft.EntityFrameworkCore;
using ServidorApi.Models;
 
namespace ServidorApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
 
        public DbSet<Usuario> Usuarios { get; set; }
 
        // Agora representa conta de sistema, não conta financeira
        public DbSet<Conta> Contas { get; set; }
 
        public DbSet<Entradas> Entrada { get; set; }
        public DbSet<Saida> Saidas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
 
            // Mantém o mapeamento explícito da tabela de entrada
            modelBuilder.Entity<Entradas>().ToTable("tb_entrada");
 
            // Configura relação 1 para 1 entre Usuario e Conta
            // Um usuário tem exatamente uma conta de sistema
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Conta)               // Usuario tem uma Conta
                .WithOne(c => c.Usuario)             // Conta pertence a um Usuario
                .HasForeignKey<Conta>(c => c.UsuarioID) // FK está em Conta
                .OnDelete(DeleteBehavior.Cascade);   // Deletar usuário deleta a conta
        }
    }
}
 
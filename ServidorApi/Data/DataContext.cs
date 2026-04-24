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
        
        // As próximas tabelas (Conta, Entrada, Saida) você adicionará aqui embaixo
    }
}
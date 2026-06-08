using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 
namespace ServidorApi.Models
{
    [Table("tb_usuario")]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int ID { get; set; }
 
        [Required]
        [Column("nome_usuario")]
        [StringLength(50)]
        public string Nome { get; set; } = string.Empty;
 
        [Required]
        [Column("sobreNome_usuario")]
        [StringLength(100)]
        public string Sobrenome { get; set; } = string.Empty;
 
        [Required]
        [Column("idade_usuario")]
        public int Idade { get; set; }
 
        [Required]
        [Column("telefone_usuario")]
        [StringLength(15)]
        public string Telefone { get; set; } = string.Empty;
 
        // Navegação 1 para 1 — cada usuário tem exatamente uma conta de sistema
        // É criada automaticamente junto com o usuário no UsuarioService
        public virtual Conta? Conta { get; set; }
    }
}
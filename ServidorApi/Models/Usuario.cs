using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServidorApi.Models
{
    [Table ("tb_usuario")]
    public class Usuario
    {
        [Key]
        [Column ("id_usuario")]
        public int UsuarioID {get; set; }

        [Required]
        [Column ("nome_usuario")]
        [StringLength(50)]
        public string Nome {get; set; } = string.Empty;

        [Required]
        [Column ("sobreNome_usuario")]
        [StringLength(100)]
        public string Sobrenome {get; set; } = string.Empty;

        [Required]
        [Column ("idade_usuario")]
        public int Idade {get; set; }

        [Required]
        [Column ("telefone_usuario")]
        [StringLength(15)]  
        public string Telefone {get; set; } = string.Empty;

        [Required]
        [Column ("eMail_usuario")]
        [StringLength(80)]  
        public string Email {get; set; } = string.Empty;
        
        [Required]
        [Column ("senha_usuario")] 
        [StringLength(255)] 
        public string Senha {get; set; } = string.Empty;

        [Required]
        [Column ("dataCriacao_usuario")]  
        public DateTime DataCriacao {get; set; } = DateTime.Now;
    }
}
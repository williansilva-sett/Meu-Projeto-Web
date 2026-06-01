using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServidorApi.Models
{
    [Table ("tb_usuario")]
    public class Usuario
    {
        [Key]
        [Column ("id_usuario")]
        public int ID {get; set; }

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
        [Column ("tipo_usuario")]
        public TipoUsuario Tipo { get; set; } = TipoUsuario.Usuario;

        [Required]
        [Column ("dataCriacao_usuario")]  
        public DateTime DataCriacao {get; set; } = DateTime.Now;

        [Required]
        [Column("ativo_usuario")]
        public bool Ativo { get; set; } = true; // Novos usuários nascem ativos por padrão
        // Contador de tentativas de login falhas consecutivas
        // Volta a zero após login bem-sucedido ou após o bloqueio expirar
        [Column("tentativas_login")]
        public int TentativasLogin { get; set; } = 0;
        // Data/hora até quando a conta está bloqueada
        // null = conta liberada | com valor = bloqueada até aquele momento
        [Column("bloqueado_ate")]
        public DateTime? BloqueadoAte { get; set; }

        public virtual ICollection<Conta> Contas { get; set; } = new List<Conta>();
    }

    
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 
namespace ServidorApi.Models
{
    [Table("tb_conta")]
    public class Conta
    {
        [Key]
        [Column("id_conta")]
        public int ID { get; set; }
 
        // Email usado para login — deve ser único no sistema
        [Required]
        [Column("email_conta")]
        [StringLength(80)]
        public string Email { get; set; } = string.Empty;
 
        // Hash BCrypt da senha — nunca armazena a senha pura
        [Required]
        [Column("senha_conta")]
        [StringLength(255)]
        public string Senha { get; set; } = string.Empty;
 
        // Define se é usuário comum ou administrador
        // Salvo como inteiro no banco (0 = Usuario, 1 = Admin)
        [Required]
        [Column("tipo_conta")]
        public TipoUsuario Tipo { get; set; } = TipoUsuario.Usuario;
 
        // Se false, o usuário não consegue fazer login mesmo com credenciais corretas
        [Required]
        [Column("ativo_conta")]
        public bool Ativo { get; set; } = true;
 
        // Contador de tentativas de login falhas consecutivas
        // Zerado após login bem-sucedido ou após o bloqueio expirar
        [Column("tentativas_login")]
        public int TentativasLogin { get; set; } = 0;
 
        // null = conta liberada | com valor = bloqueada até aquele momento UTC
        [Column("bloqueado_ate")]
        public DateTime? BloqueadoAte { get; set; }
 
        // Data de criação da conta de sistema
        [Required]
        [Column("data_criacao_conta")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;
 
        // FK para o usuário dono desta conta — relação 1 para 1
        [Required]
        [Column("id_usuario")]
        public int UsuarioID { get; set; }
 
        // Propriedade de navegação — permite acessar os dados pessoais do usuário
        // via conta.Usuario.Nome, conta.Usuario.Telefone, etc.
        [ForeignKey("UsuarioID")]
        public virtual Usuario? Usuario { get; set; }
    }
}
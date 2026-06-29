using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServidorApi.Models
{
    [Table("tb_token_recuperacao_senha")]
    public class TokenRecuperacaoSenha
    {
        [Key]
        [Column("id_token")]
        public int ID { get; set; }

        [Required]
        [Column("token")]
        [StringLength(100)]
        public string Token { get; set; } = string.Empty;

        [Column("conta_id")]
        public int ContaID { get; set; }

        [ForeignKey(nameof(ContaID))]
        public Conta? Conta { get; set; }

        [Column("data_criacao")]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        [Column("data_expiracao")]
        public DateTime DataExpiracao { get; set; }

        [Column("usado")]
        public bool Usado { get; set; } = false;
    }
}
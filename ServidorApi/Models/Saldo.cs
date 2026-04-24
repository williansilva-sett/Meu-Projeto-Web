using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServidorApi.Models
{
    [Table ("tb_saldo_conta")]
    public class Saldo_Conta
    {
        [Key]
        [Column ("id_conta")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IDConta {get; set; }

        [Required]
        [Column ("total_entrada_conta", TypeName = "decimal(10,2)")]
        public decimal TotalEntrada {get; set; }

        [Required]
        [Column ("total_saida_conta", TypeName = "decimal(10,2)")]
        public decimal TotalSaida {get; set; }

        [Required]
        [Column ("saldo_atual_conta", TypeName = "decimal(10,2)")]
        public decimal SaldoAtual {get; set; }

        [Required]
        [Column ("ultima_atualizacao_conta")]
        public DateTime UltimaAtualizacao {get; set; } = DateTime.Now;

        [ForeignKey ("IDConta")]
        public virtual Conta? Conta {get; set; }
    }
}
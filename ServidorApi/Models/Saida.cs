using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.SignalR;

namespace ServidorApi.Models
{
    [Table ("tb_saida")]
    public class Saida
    {
        [Key]
        [Required]
        [Column ("id_saida")]
        public int IDSaida {get; set; }

        [Required]
        [Column ("valor_saida", TypeName = "decimal(10,2)")]
        public decimal Valor_saida {get; set; }

        [Required]
        [Column ("data_saida")]
        public DateTime Data_saida {get; set; } = DateTime.Now;

        [Required]
        [Column ("id_conta")]
        public int IDConta {get; set; }

        [ForeignKey ("IDConta")]
        public virtual Conta? Conta {get; set; }

        [Required]
        [Column ("id_categoria")]
        public int IDCategoria {get; set; }

        [ForeignKey ("IDCategoria")]
        public virtual Categoria? Categoria {get; set; }
    }
}
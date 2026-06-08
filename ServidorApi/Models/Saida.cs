using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 
namespace ServidorApi.Models
{
    [Table("tb_saida")]
    public class Saida
    {
        [Key]
        [Required]
        [Column("id_saida")]
        public int IDSaida { get; set; }
 
        [Required]
        [Column("valor_saida", TypeName = "decimal(10,2)")]
        public decimal ValorSaida { get; set; }
 
        [Required]
        [Column("data_saida")]
        public DateTime DataSaida { get; set; } = DateTime.Now;
 
        // FK para Usuario — substituiu a FK para Conta financeira
        [Required]
        [Column("id_usuario")]
        public int IDUsuario { get; set; }
 
        [ForeignKey("IDUsuario")]
        public virtual Usuario? Usuario { get; set; }
 
        [Required]
        [Column("id_categoria")]
        public int IDCategoria { get; set; }
 
        [ForeignKey("IDCategoria")]
        public virtual Categoria? Categoria { get; set; }
    }
}
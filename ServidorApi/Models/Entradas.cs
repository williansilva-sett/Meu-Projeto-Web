using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 
namespace ServidorApi.Models
{
    [Table("tb_entrada")]
    public class Entradas
    {
        [Key]
        [Required]
        [Column("id_entrada")]
        public int IDEntrada { get; set; }
 
        [Required]
        [Column("descricao_entrada")]
        [StringLength(100)]
        public string Descricao { get; set; } = string.Empty;
 
        [Required]
        [Column("valor_entrada", TypeName = "decimal(10,2)")]
        public decimal ValorEntrada { get; set; }
 
        [Required]
        [Column("data_entrada")]
        public DateTime Data { get; set; } = DateTime.Now;
 
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
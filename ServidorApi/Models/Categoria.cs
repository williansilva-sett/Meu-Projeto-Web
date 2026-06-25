using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServidorApi.Models
{
    [Table ("tb_categoria")]
    public class Categoria
    {
        [Key]
        [Column ("id_categoria")]
        public int IDCategoria {get; set; }

        [Column ("nome_categoria")]
        [StringLength(80)]
        public string categoria {get; set; } = string.Empty;

        [Required]
        [Column ("tipo_categoria")]
        public TipoCategoria Tipo { get; set; }
        
        [Column ("id_usuario")]
        public int? IDUsuario { get; set; }
    }
}
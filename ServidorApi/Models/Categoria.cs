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
        public string Nome {get; set; } = string.Empty;

        [Column ("tipo_categoria")]
        [StringLength(7)]
        public string Tipo {get; set; } = string.Empty;
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServidorApi.Models
{
    [Table ("tb_conta")]
    public class Conta
    {
        [Key]
        [Column  ("id_conta")]
        public int IDConta {get; set; }

        [Required]
        [Column ("nome_conta")]
        [StringLength(45)]
        public string NomeConta {get; set; } = string.Empty;

        [Column ("ativa_conta")]
        public bool Ativa {get; set; }

        [Required]
        [Column ("id_usuario")]
        public int UsuarioID {get; set; }

        [ForeignKey ("UsuarioID")]
        public virtual Usuario? Usuario {get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 
namespace ServidorApi.Models
{
    // Opções de status da meta
    public enum StatusMeta
    {
        EmAndamento = 0,  // Meta ainda não atingida
        Concluida   = 1,  // Valor atual atingiu o valor alvo
        Cancelada   = 2   // Meta cancelada pelo usuário
    }
 
    [Table("tb_meta")]
    public class Meta
    {
        [Key]
        [Column("id_meta")]
        public int ID { get; set; }
 
        // Nome da meta — ex: "Viagem para Europa", "Reserva de emergência"
        [Required]
        [Column("nome_meta")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;
 
        // Descrição opcional com mais detalhes sobre a meta
        [Column("descricao_meta")]
        [StringLength(255)]
        public string? Descricao { get; set; }
 
        // Quanto o usuário quer acumular
        [Required]
        [Column("valor_alvo_meta", TypeName = "decimal(10,2)")]
        public decimal ValorAlvo { get; set; }
 
        // Quanto já foi acumulado — atualizado manualmente ou via entradas
        [Required]
        [Column("valor_atual_meta", TypeName = "decimal(10,2)")]
        public decimal ValorAtual { get; set; } = 0;
 
        // Quando a meta foi criada
        [Required]
        [Column("data_inicio_meta")]
        public DateTime DataInicio { get; set; } = DateTime.Now;
 
        // Prazo para atingir a meta — nullable pois pode ser sem prazo
        [Column("data_limite_meta")]
        public DateTime? DataLimite { get; set; }
 
        // Status atual da meta
        [Required]
        [Column("status_meta")]
        public StatusMeta Status { get; set; } = StatusMeta.EmAndamento;
 
        // FK para o usuário dono da meta
        [Required]
        [Column("id_usuario")]
        public int IDUsuario { get; set; }
 
        [ForeignKey("IDUsuario")]
        public virtual Usuario? Usuario { get; set; }
    }
}
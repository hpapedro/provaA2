using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoAPI.Models
{
    public class Evento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(100)]
        public string Local { get; set; }

        [Required]
        public DateTime Data { get; set; }

        public DateTime CriadoEm { get; set; } = DateTime.Now;

        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        [InverseProperty("Eventos")]
        public Usuario Usuario { get; set; }
    }
}

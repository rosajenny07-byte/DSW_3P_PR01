
using System.ComponentModel.DataAnnotations;

namespace DSW_3P_PR01.Models
{
    public class Mision
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dificultad es obligatoria.")]
        [RegularExpression(
            "^(Fácil|Media|Difícil)$",
            ErrorMessage = "La dificultad debe ser Fácil, Media o Difícil."
        )]
        public string Dificultad { get; set; } = string.Empty;

        [Range(0, int.MaxValue,
            ErrorMessage = "La recompensa XP no puede ser negativa.")]
        public int RecompensaXP { get; set; }

        public bool Completada { get; set; } = false;
    }
}

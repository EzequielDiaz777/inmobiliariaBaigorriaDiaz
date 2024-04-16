using System.ComponentModel.DataAnnotations;

namespace inmobiliariaBaigorriaDiaz.Models
{
    public class Registro
    {
        [Display(Name = "ID del registro")]
        public int IdRegistro { get; set; }
        [Required]
        [Display(Name = "ID del usuario")]
        public int IdUsuario { get; set; }
        [Required]
        public int IdFila { get; set; } 
        [Display(Name = "Teléfono")]
        public string? NombreDeTabla { get; set; }
        [EmailAddress]
        public string? TipoDeAccion { get; set; }
        [Required]
        public DateOnly FechaDeAccion { get; set; }
        public Usuario usuario {get; set;} = new Usuario();
    }
}
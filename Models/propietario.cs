using System.ComponentModel.DataAnnotations;

namespace inmobiliariaBaigorriaDiaz.Models
{
    public class Propietario
    {
        [Display(Name = "ID del propietario")]
        public int IdPropietario { get; set; }
        [Required]
        public string Nombre { get; set; } = "";
        [Required]
        [Display(Name = "Apellido del propietario")]
        public string Apellido { get; set; } = "";
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string DNI { get; set; } = "";
        [Required]
        [Display(Name = "Estado del propietario")]
        public bool Estado { get; set; }
        public override string ToString()
        {
            return $"{Apellido} {Nombre}";
        }
    }
}
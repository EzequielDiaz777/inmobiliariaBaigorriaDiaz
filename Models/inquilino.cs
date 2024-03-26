using System.ComponentModel.DataAnnotations;

namespace inmobiliariaBaigorriaDiaz.Models
{
    public class Inquilino
    {
        [Display(Name = "Id de inquilino")]
        public int IdInquilino { get; set; }
        [Required]
        public string Nombre { get; set; } = "";
        [Required]
        public string Apellido { get; set; } = "";
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string DNI { get; set; } = "";
        [Required]
        [Display(Name = "Estado del inquilino")]
        public bool Estado { get; set; }
        public override string ToString()
        {
            return $"{Apellido} {Nombre}";
        }
    }
}
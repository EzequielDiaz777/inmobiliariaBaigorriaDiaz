using System.ComponentModel.DataAnnotations;

namespace inmobiliariaBaigorriaDiaz.Models
{
    public class TipoDeInmueble
    {
        [Display(Name = "Código")]
        public int IdTipoDeInmueble { get; set; }
        [Required]
        [Display(Name = "Tipo de inmueble")]
        public string Nombre { get; set; } = "";
        [Display(Name = "Disponible")]
        public bool Estado { get; set; }
    }
}
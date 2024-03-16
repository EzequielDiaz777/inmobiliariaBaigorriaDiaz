using System.ComponentModel.DataAnnotations;

namespace inmobiliariaBaigorriaDiaz.Models
{
    public class TipoDeInmueble
    {
        [Display(Name = "Código del tipo de inmueble")]
        public int IdTipoDeInmueble { get; set; }
        [Required]
        [Display(Name = "Tipo de inmueble")]
        public string Tipo { get; set; } = "";
        [Display(Name = "Disponible")]
        public bool Estado { get; set; }
        /*public override string ToString()
        {
            return $"{Tipo}";
        }*/
    }
}
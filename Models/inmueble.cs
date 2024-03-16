using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliariaBaigorriaDiaz.Models
{
    public class Inmueble
    {
        [Display(Name = "Nº de inmueble")]
        public int IdInmueble { get; set; }

        [Required]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = "";

        [Required]
        [Display(Name = "Tipo de inmueble")]
        public int IdTipoDeInmueble { get; set; }

        [Required]
        [ForeignKey(nameof(IdTipoDeInmueble))]
        public TipoDeInmueble TipoDeInmueble { get; set; } = new TipoDeInmueble();

        [Required]
        public int Ambientes { get; set; } = 0;

        [Required]
        public decimal Superficie { get; set; } = 0;

        public string? Latitud { get; set; }

        public string? Longitud { get; set; }

        [Required]
        public decimal Precio { get; set; }

        [Required]
        public bool Estado { get; set; }

        [Display(Name = "Dueño")]
        [Required]
        public int IdPropietario { get; set; }

        [Required]
        [ForeignKey(nameof(IdPropietario))]
        public Propietario Duenio { get; set; } = new Propietario();
    }
}
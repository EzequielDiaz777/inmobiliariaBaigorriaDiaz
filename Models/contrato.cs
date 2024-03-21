using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliariaBaigorriaDiaz.Models
{
    public class Contrato
    {
        [Display(Name = "N° de contrato")]
        public int IdContrato { get; set; }

        [Required]
        [Display(Name = "Inquilino")]
        public int IdInquilino { get; set; }

        [Required]
        [Display(Name = "Inmueble")]
        public int IdInmueble { get; set; }
        [Required]
        [Display(Name = "Precio del contrato")]
        public decimal PrecioDeContrato { get; set; }
        [Required]
        public DateOnly AlquilerDesde { get; set; }

        [Required]
        public DateOnly AlquilerHasta { get; set; }
        [Required]
        [Display(Name = "Contrato vigente")]
        public bool Estado { get; set; }
        [Required]
        [ForeignKey(nameof(IdInquilino))]
        public Inquilino Inquilino { get; set; } = new Inquilino();

        [Required]
        [ForeignKey(nameof(IdInmueble))]
        public Inmueble Inmueble { get; set; } = new Inmueble();


    }
}
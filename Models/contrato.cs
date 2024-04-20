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
        public decimal Precio { get; set; }
        [Required]
        [Display(Name = "Alquiler desde")]
        public DateOnly AlquilerDesde { get; set; }
        [Required]
        [Display(Name = "Alquiler hasta")]
        public DateOnly AlquilerHasta { get; set; }
        [Required]
        [Display(Name = "Fecha original del final del alquiler")]
        public DateOnly AlquilerHastaOriginal { get; set; }
        
        [Required]
        [ForeignKey(nameof(IdInquilino))]
        [Display(Name = "Nombre del inquilino")]
        public Inquilino Inquilino { get; set; } = new Inquilino();

        [Required]
        [ForeignKey(nameof(IdInmueble))]
        [Display(Name = "Dirección del inmueble")]
        public Inmueble Inmueble { get; set; } = new Inmueble();


    }
}
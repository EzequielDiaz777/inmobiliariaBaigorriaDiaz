using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliariaBaigorriaDiaz.Models;

public class Inmueble
{
    [Key]
    [Display(Name = "Nº de Inmueble")]
    public int IdInmueble { get; set; }

    [Required]
    [Display(Name = "Tipo")]
    public int IdTipoDeInmueble { get; set; }

    [ForeignKey(nameof(IdTipoDeInmueble))]
    public TipoDeInmueble Tipo { get; set; } = new TipoDeInmueble();

    [Required]
    [Display(Name = "Uso")]
    public int IdUsoDeInmueble { get; set; }

    [ForeignKey(nameof(IdUsoDeInmueble))]
    public UsoDeInmueble Uso { get; set; } = new UsoDeInmueble();

    [Required]
    [Display(Name = "Dirección")]
    public string Direccion { get; set; } = "";

    [Required]
    public int Ambientes { get; set; }

    [Required]
    public decimal Superficie { get; set; }

    public decimal? Longitud { get; set; }

    public decimal? Latitud { get; set; }

    [Required]
    public decimal Precio { get; set; }

    [Required]
    [Display(Name = "Estado del inmueble")]
    public bool Estado { get; set; }

    [Display(Name = "Dueño")]
    public int IdPropietario { get; set; }

    [ForeignKey(nameof(IdPropietario))]
    [Display(Name = "Propietario")]
    public Propietario Duenio { get; set; } = new Propietario();

}
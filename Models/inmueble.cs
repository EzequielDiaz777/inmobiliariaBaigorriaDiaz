using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace inmobiliariaBaigorriaDiaz.Models;

public class Inmueble
{
    [Key]
    [Display(Name = "Nº")]
    public int IdInmueble { get; set; }

    [Required]
    [Display(Name = "Tipo")]
    public int IdTipoDeInmueble { get; set; }

    [ForeignKey(nameof(IdTipoDeInmueble))]
    public TipoDeInmueble Tipo { get; set; } = new TipoDeInmueble();

    [Required]
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
    public string Uso { get; set; } = "";
    
    [Required]
    public bool Estado { get; set;}

    [Display(Name = "Dueño")]
    public int IdPropietario { get; set; }

    [ForeignKey(nameof(IdPropietario))]
    public Propietario Duenio { get; set; } = new Propietario();

}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace inmobiliariaBaigorriaDiaz.Models;

public class Pago
{
    [Key]
    [Display(Name = "NumeroDePago")]
    public int NumeroDePago { get; set; }

    [Display(Name = "Id")]
    public int IdContrato { get; set; }

    [ForeignKey(nameof(IdContrato))]
    public Contrato Contrato { get; set; } = new Contrato();

    [Required]
    [Display(Name = "Monto")]
    public decimal Monto { get; set; }

    [Required]
    [Display(Name = "Fecha")]
    public DateOnly Fecha { get; set; }
}  
    

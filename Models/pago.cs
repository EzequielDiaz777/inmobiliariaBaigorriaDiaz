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
    [Display(Name = "Número de pago")]
    public int NumeroDePago { get; set; }

    [Display(Name = "Número de contrato")]
    public int IdContrato { get; set; }

    [ForeignKey(nameof(IdContrato))]
    public Contrato Contrato { get; set; } = new Contrato();

    [Required]
    [Display(Name = "Monto del pago")]
    public decimal Monto { get; set; }

    [Required]
    [Display(Name = "Fecha de pago")]
    public DateOnly Fecha { get; set; }
}  
    

using System.ComponentModel.DataAnnotations;

namespace inmobiliariaBaigorriaDiaz.Models
{
	public class UsoDeInmueble
	{
		[Display(Name = "Código")]
		public int IdUsoDeInmueble { get; set; }
		[Required]
		[Display(Name = "Uso del inmueble")]
		public string Nombre { get; set; } = "";
		[Required]
		[Display(Name = "Disponible")]
		public bool Estado {get; set;} = true;
	}
}
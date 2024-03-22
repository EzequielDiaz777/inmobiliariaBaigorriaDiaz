using System.ComponentModel.DataAnnotations;

namespace inmobiliariaBaigorriaDiaz.Models
{
	public class UsoDeInmueble
	{
		[Display(Name = "Código")]
		public int IdUsoDeInmueble { get; set; }
		[Required]
		public string Uso { get; set; } = "";
		[Required]
		public bool Estado {get; set;} = true;
	}
}
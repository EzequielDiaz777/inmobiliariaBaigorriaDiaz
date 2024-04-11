using System.ComponentModel.DataAnnotations;

namespace inmobiliariaBaigorriaDiaz.Models
{
    public enum enRoles
    {
        Administrador = 1,
        Empleado = 2,
    }

    public class Usuario
    {
        [Key]
        [Display(Name = "Código")]
        public int IdUsuario { get; set; }

        [Required]
        public string Nombre { get; set; } = "";

        [Required]
        public string Apellido { get; set; } = "";

        [Required]
        public string Email { get; set; } = "";

        [Required, DataType(DataType.Password)]
        public string Clave { get; set; } = "";

        public string? AvatarURL { get; set; } = "";
        
        [Required]
        public int Rol { get; set; }
        
        public string RolNombre => Rol > 0 ? ((enRoles)Rol).ToString() : "";
        
        public bool Estado {get; set;}
        
        public IFormFile? AvatarFile { get; set; }

        public override string ToString()
        {
            return $"{Apellido} {Nombre}";
        }

        public static IDictionary<int, string> ObtenerRoles()
        {
            SortedDictionary<int, string> roles = new SortedDictionary<int, string>();
            Type tipoEnumRol = typeof(enRoles);
            foreach (var valor in Enum.GetValues(tipoEnumRol))
            {
                roles.Add((int)valor, Enum.GetName(tipoEnumRol, valor));
            }
            return roles;
        }

    }
}
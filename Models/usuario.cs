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
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = "";

        [Required]
        public string Apellido { get; set; } = "";

        [Required]
        public string Email { get; set; } = "";

        [Required, DataType(DataType.Password)]
        public string Clave { get; set; } = "";
        
        public string AvatarUrl { get; set; } = "";
        [NotMapped]

        public int Rol { get; set; }
        [NotMapped]

        public string RolNombre => Rol > 0 ? ((enRoles)Rol).ToString() : "";

        public IFormFile AvatarFile { get; set; }

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
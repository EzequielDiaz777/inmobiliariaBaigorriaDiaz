using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace inmobiliariaBaigorriaDiaz.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IWebHostEnvironment environment;
        private RepositorioUsuario ru = new RepositorioUsuario();

        public UsuarioController(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }


        // GET: Usuario
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public ActionResult Index()
        {
            ViewBag.Id = TempData["Id"];
            ViewBag.entidad = "usuario";
            return View(ru.ObtenerUsuarios());
        }

        // GET: Usuario/Details/5
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public ActionResult Details(int id)
        {
            return View(ru.ObtenerUsuarioPorID(id));
        }

        // GET: Usuario/Create
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View();
        }

        [HttpGet]
        public ActionResult Alta(int id)
        {
            return View(ru.ObtenerUsuarioPorID(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Alta(int id, IFormCollection collection)
        {
            try
            {
                ru.AltaLogica(id);
                return RedirectToAction(nameof(Index));

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                ModelState.AddModelError(string.Empty, "Ha ocurrido un error al cargar la página.");
                return View(); // Retorna la vista con el modelo para que el usuario pueda corregir la entrada.
            }
        }

        // GET: Usuario/EditAdministrador/5
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public ActionResult EditAdministrador(int id)
        {
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View(ru.ObtenerUsuarioPorID(id));
        }

        [HttpPost]
        public async Task<ActionResult> EditAdministrador(int id, Usuario usuario, IFormFile? avatarFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Itera sobre cada par clave-valor en ModelState
                    foreach (var entry in ModelState)
                    {
                        // Obtiene el nombre de la propiedad
                        var propertyName = entry.Key;

                        // Obtiene la colección de errores para la propiedad actual
                        var errors = entry.Value.Errors;

                        // Itera sobre los errores en la colección
                        foreach (var error in errors)
                        {
                            // Accede a la descripción del error
                            var errorMessage = error.ErrorMessage;

                            // Haz lo que necesites con el mensaje de error
                            Console.WriteLine($"Error en la propiedad '{propertyName}': {errorMessage}");
                        }
                    }
                    return View();
                }
                else
                {
                    Usuario? u = ru.ObtenerUsuarioPorID(id);
                    if (u != null)
                    {
                        u.Nombre = usuario.Nombre;
                        u.Apellido = usuario.Apellido;
                        u.Email = usuario.Email;
                        u.Rol = usuario.Rol;
                        if (usuario.Clave != null)
                        {
                            u.Clave = Usuario.hashearClave(usuario.Clave);
                        }
                        if (avatarFile != null)
                        {
                            var resizedImagePath = await ProcesarAvatarAsync(u, avatarFile);
                            if (resizedImagePath == null)
                            {
                                return View(usuario); // Retorna la vista con errores de validación si la extensión del archivo no es válida
                            }
                            // Actualizar la URL del avatar en la base de datos
                            u.AvatarURL = resizedImagePath;
                        }
                        ru.Modificacion(u);
                        return RedirectToAction("Index"); // Redirige al Index después de una edición exitosa

                    }
                    else
                    {
                        ViewBag.Roles = Usuario.ObtenerRoles();
                        return View(ru.ObtenerUsuarioPorID(id));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return RedirectToAction("EditAdministrador", new { id = id });
            }
        }

        private async Task<string?> ProcesarAvatarAsync(Usuario usuario, IFormFile avatarFile)
        {
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".jfif", ".bmp" };
            var extension = Path.GetExtension(avatarFile.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("AvatarFile", "Solo se permiten archivos de imagen (JPG, JPEG, PNG, GIF, JFIF, BMP).");
                return null; // Retorna null si la extensión del archivo no es válida
            }

            var directoryPath = Path.Combine(environment.WebRootPath, "update");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Renombrar el archivo y obtener su nueva ruta
            var avatarFileName = $"avatar_{usuario.IdUsuario}{extension}";
            var avatarFilePath = Path.Combine(directoryPath, avatarFileName);

            // Guardar el archivo en el directorio 'update'
            using (var stream = new FileStream(avatarFilePath, FileMode.Create))
            {
                await avatarFile.CopyToAsync(stream);
            }


            // Redimensionar la imagen antes de guardarla
            var resizedImagePath = ResizeImage(avatarFilePath);

            // Retorna la ruta de la imagen redimensionada
            return resizedImagePath;
        }

        private string ResizeImage(string imagePath)
        {
            using (var image = Image.Load(imagePath))
            {
                image.Mutate(x => x.Resize(30, 30));
                var resizedImagePath = Path.Combine(environment.WebRootPath, "update", Path.GetFileName(imagePath));
                image.Save(resizedImagePath);
                return Path.Combine("/update", Path.GetFileName(imagePath));
            }
        }


        // POST: Usuario/Create
        [HttpPost]
        public async Task<ActionResult> Create(Usuario usuario, IFormFile? avatarFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    usuario.Clave = Usuario.hashearClave(usuario.Clave);
                    var nbreRnd = Guid.NewGuid();
                    ru.AltaFisica(usuario);

                    if (avatarFile != null)
                    {
                        var resizedImagePath = await ProcesarAvatarAsync(usuario, avatarFile);
                        if (resizedImagePath == null)
                        {
                            return View(usuario); // Retorna la vista con errores de validación si la extensión del archivo no es válida
                        }

                        // Actualizar la ruta de la imagen redimensionada en la propiedad AvatarURL del usuario
                        usuario.AvatarURL = resizedImagePath;

                        ru.Modificacion(usuario);
                        TempData["Id"] = usuario.IdUsuario;
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    return View(usuario);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return View();
            }
        }

        private bool mismoUsuario(int id)
        {
            // Obtener el ID del usuario autenticado
            var usuarioId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.PrimarySid));

            // Verificar si el usuario autenticado coincide con el ID del perfil solicitado
            return usuarioId != id;
        }

        // GET: Usuario/EditPerfil/5
        [HttpGet]
        public ActionResult EditPerfil(int id)
        {

            if (mismoUsuario(id))
            {
                return RedirectToAction("Perfil", "Home");
            }

            ViewBag.Roles = Usuario.ObtenerRoles();
            return View(ru.ObtenerUsuarioPorID(id));
        }

        // POST: Usuario/EditPerfil/5
        [HttpPost]
        public ActionResult EditPerfil(int id, Usuario usuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Usuario u = ru.ObtenerUsuarioPorID(id);
                    if (u != null)
                    {
                        u.Nombre = usuario.Nombre;
                        u.Apellido = usuario.Apellido;
                        u.Email = usuario.Email;
                        ru.Modificacion(u);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        // Si el modelo no es válido, vuelve a mostrar la vista de edición de perfil con los errores de validación
                        ViewBag.Roles = Usuario.ObtenerRoles();
                        return View(usuario);
                    }
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return RedirectToAction("EditPerfil", new { id = id });
            }
        }

        // GET: Usuario/EditAvatar/5
        [HttpGet]
        public ActionResult EditAvatar(int id)
        {
            if (mismoUsuario(id))
            {
                return RedirectToAction("Perfil", "Home");
            }
            return View(ru.ObtenerUsuarioPorID(id));
        }

        // POST: Usuario/EditAvatar/5
        [HttpPost]
        public async Task<ActionResult> EditAvatar(int id, IFormFile? avatarFile)
        {
            try
            {
                var usuario = ru.ObtenerUsuarioPorID(id);
                if (avatarFile != null)
                {
                    var resizedImagePath = await ProcesarAvatarAsync(usuario, avatarFile);
                    if (resizedImagePath == null)
                    {
                        return View(usuario); // Retorna la vista con errores de validación si la extensión del archivo no es válida
                    }

                    // Actualizar la URL del avatar en la base de datos
                    usuario.AvatarURL = resizedImagePath;
                    ru.Modificacion(usuario);

                    TempData["Id"] = usuario.IdUsuario;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Si no se proporciona un archivo, simplemente redirige a la página de índice
                    usuario.AvatarURL = "";
                    ru.Modificacion(usuario);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return RedirectToAction("EditAvatar", new { id = id });
            }
        }

        // GET: Usuario/EditClave/5
        [HttpGet]
        public ActionResult EditClaveEmpleado(int id)
        {
            if (mismoUsuario(id))
            {
                return RedirectToAction("Perfil", "Home");
            }
            return View();
        }

        // POST: Usuario/EditClave/5
        [HttpPost]
        public ActionResult EditClaveEmpleado(string claveVieja, string nuevaClave, string confirmarNuevaClave, int id)
        {
            try
            {
                // Validar que los datos ingresados sean válidos
                if (string.IsNullOrWhiteSpace(claveVieja) || string.IsNullOrWhiteSpace(nuevaClave) || string.IsNullOrWhiteSpace(confirmarNuevaClave))
                {
                    // Si los datos no son válidos, agregar errores al modelo de estado
                    ModelState.AddModelError("", "No pueden dejarse campos vacíos.");
                    return View();
                }
                else
                {
                    var usuario = ru.ObtenerUsuarioPorID(id);
                    var claveViejaHasheada = Usuario.hashearClave(claveVieja);
                    if (!claveViejaHasheada.Equals(usuario.Clave))
                    {
                        Console.WriteLine("Clave vieja: " + usuario.Clave);
                        Console.WriteLine("Clave nueva: " + Usuario.hashearClave(claveVieja));
                        Console.WriteLine("Claves iguales: " + usuario.Clave == Usuario.hashearClave(claveVieja));
                        ModelState.AddModelError("", "La clave vieja ingresada es incorrecta.");
                        return View();
                    }
                    else if (!nuevaClave.Equals(confirmarNuevaClave))
                    {
                        ModelState.AddModelError("", "Las nuevas claves ingresadas no coinciden.");
                        return View();
                    }
                    usuario.Clave = Usuario.hashearClave(nuevaClave);
                    ru.Modificacion(usuario);

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return RedirectToAction("EditClaveEmpleado");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public ActionResult EditClaveAdministrador(int id)
        {
            return View();
        }

        // POST: Usuario/EditClave/5
        [HttpPost]
        public ActionResult EditClaveAdministrador(string clave, int id)
        {
            try
            {
                var usuario = ru.ObtenerUsuarioPorID(id);
                usuario.Clave = Usuario.hashearClave(clave);
                ru.Modificacion(usuario);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return RedirectToAction("EditClaveAdministrador");
            }
        }

        // GET: Usuario/Delete/5
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int id)
        {
            return View(ru.ObtenerUsuarioPorID(id));
        }

        // POST: Usuario/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                ru.BajaLogica(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: /Usuarios/Login
        public ActionResult LoginModal()
        {
            return PartialView("_LoginModal", new LoginView());
        }

        public IActionResult NoAutorizado()
        {
            return View(); // Puedes redirigir a una vista específica para mostrar un mensaje de error o realizar alguna acción.
        }

        // Filtro de acción para redirigir si el usuario no está autenticado
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!User.Identity.IsAuthenticated)
            {
                context.Result = RedirectToAction("Index", "Home"); // Redirige al Index del controlador Home
            }
            base.OnActionExecuting(context);
        }
    }
}
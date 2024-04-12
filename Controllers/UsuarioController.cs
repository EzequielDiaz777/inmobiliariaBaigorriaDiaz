using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;

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
        public ActionResult Index()
        {
            ViewBag.Id = TempData["Id"];
            ViewBag.entidad = "usuario";
            return View(ru.ObtenerUsuarios());
        }

        // GET: Usuario/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            return View(ru.ObtenerUsuarioPorID(id));
        }

        // GET: Usuario/Create
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View();
        }

        // POST: Usuario/Create
        [HttpPost]
        public async Task<ActionResult> Create(Usuario usuario, IFormFile avatarFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (avatarFile != null)
                    {
                        string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                        var extension = Path.GetExtension(avatarFile.FileName).ToLower();
                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("AvatarFile", "Solo se permiten archivos de imagen (JPG, JPEG, PNG, GIF).");
                            return View(usuario);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("AvatarFile", "Debe seleccionar un archivo.");
                        return View(usuario);
                    }

                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: usuario.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes("inmobiliariaBaigorriaDiaz"),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 10000,
                        numBytesRequested: 256 / 8
                    ));
                    usuario.Clave = hashed;
                    var nbreRnd = Guid.NewGuid();
                    ru.AltaFisica(usuario);

                    var directoryPath = Path.Combine(environment.WebRootPath, "update");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // Renombrar el archivo y obtener su nueva ruta
                    var avatarFileName = $"avatar_{usuario.IdUsuario}{Path.GetExtension(avatarFile.FileName)}";
                    var avatarFilePath = Path.Combine(directoryPath, avatarFileName);

                    // Guardar el archivo en el directorio 'update'
                    using (var stream = new FileStream(avatarFilePath, FileMode.Create))
                    {
                        await avatarFile.CopyToAsync(stream);
                    }

                    // Redimensionar la imagen antes de guardarla
                    var resizedImagePath = ResizeImage(avatarFilePath, 50, 50);

                    // Actualizar la ruta de la imagen redimensionada en la propiedad AvatarURL del usuario
                    usuario.AvatarURL = resizedImagePath;

                    ru.Modificacion(usuario);
                    TempData["Id"] = usuario.IdUsuario;
                    return RedirectToAction(nameof(Index));
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

        private string ResizeImage(string imagePath, int width, int height)
        {
            using (var image = Image.Load(imagePath))
            {
                image.Mutate(x => x.Resize(width, height));
                var resizedImagePath = Path.Combine(environment.WebRootPath, "update", Path.GetFileName(imagePath));
                image.Save(resizedImagePath);
                return Path.Combine("/update", Path.GetFileName(imagePath));
            }
        }

        // GET: Usuario/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            return View(ru.ObtenerUsuarioPorID(id));
        }

        // POST: Usuario/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Usuario usuario)
        {
            try
            {
                // TODO: Add update logic here
                Usuario u = ru.ObtenerUsuarioPorID(id);
                if (u != null)
                {
                    u.Nombre = usuario.Nombre;
                    u.Apellido = usuario.Apellido;
                    u.Email = usuario.Email;
                    u.Clave = usuario.Clave;
                    u.Rol = usuario.Rol;
                    ru.Modificacion(u);
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return RedirectToAction("Edit");
            }
        }

        // GET: Usuario/Delete/5
        [HttpGet]
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
    }
}
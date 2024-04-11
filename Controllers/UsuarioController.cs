using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace inmobiliariaBaigorriaDiaz.Controllers
{
    public class UsuarioController : Controller
    {
        private RepositorioUsuario ru = new RepositorioUsuario();

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
        public async Task<ActionResult> CreateUsuario(Usuario usuario, IFormFile avatarFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
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
                    var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "update");
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

                    // Guardar la ruta de la imagen en la propiedad AvatarURL del usuario
                    usuario.AvatarURL = avatarFilePath;
                    Console.WriteLine(avatarFilePath);
                    Console.WriteLine(usuario.AvatarURL);
                    ru.Modificacion(usuario);
                    TempData["Id"] = usuario.IdUsuario;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View(usuario);
                }
            }
            catch
            {
                Console.WriteLine("Error");
                return View();
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
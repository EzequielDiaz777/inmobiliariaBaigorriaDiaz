using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Mvc;

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
            return View(rp.ObtenerUsuarios());
        }

        // GET: Usuario/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            return View(rp.ObtenerUsuarioPorID(id));
        }

        // GET: Usuario/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Usuario/Create
        [HttpPost]
        public ActionResult Create(Usuario usuario)
        {
            try
            {
                rp.AltaFisica(usuario);
                TempData["Id"] = usuario.IdUsuario;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Usuario/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            return View(rp.ObtenerUsuarioPorID(id));
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
                    u.AvatarUrl = usuario.AvatarUrl;
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
            return View(rp.ObtenerUsuarioPorID(id));
        }

        // POST: Usuario/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                rp.BajaLogica(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
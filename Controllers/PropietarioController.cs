using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace inmobiliariaBaigorriaDiaz.Controllers
{
    public class PropietarioController : Controller
    {
        private RepositorioPropietario rp = new RepositorioPropietario();

        // GET: Propietario
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Id = TempData["Id"];
            ViewBag.entidad = "propietario";
            return View(rp.ObtenerPropietarios());
        }

        // GET: Propietario/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            return View(rp.ObtenerPropietarioPorID(id));
        }

        // GET: Propietrio/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Propietario/Create
        [HttpPost]
        public ActionResult Create(Propietario propietario)
        {
            try
            {
                rp.AltaFisica(propietario);
                TempData["Id"] = propietario.IdPropietario;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Propietario/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            return View(rp.ObtenerPropietarioPorID(id));
        }

        // POST: Propietario/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Propietario propietario)
        {
            try
            {
                // TODO: Add update logic here
                Propietario p = rp.ObtenerPropietarioPorID(id);
                if (p != null)
                {
                    p.DNI = propietario.DNI;
                    p.Nombre = propietario.Nombre;
                    p.Apellido = propietario.Apellido;
                    p.Email = propietario.Email;
                    p.Telefono = propietario.Telefono;
                    p.Estado = propietario.Estado;
                    rp.Modificacion(p);
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

        // GET: Propietario/Delete/5
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int id)
        {
            if (!User.IsInRole("Administrador"))
            {
                TempData["ErrorMessage"] = "No tienes permiso para realizar esta acción.";
                return RedirectToAction(nameof(Index));
            }
            return View(rp.ObtenerPropietarioPorID(id));
        }

        // POST: Propietario/Delete/5
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
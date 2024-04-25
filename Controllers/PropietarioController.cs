using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace inmobiliariaBaigorriaDiaz.Controllers
{
    public class PropietarioController : Controller
    {
        private RepositorioPropietario rp = new RepositorioPropietario();

        // GET: Propietario
        [HttpGet]
        public ActionResult Index(int limite = 5, int paginado = 1)
        {
            // Establecer si el usuario es un administrador en ViewBag
            ViewBag.rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            // Obtener la cantidad total de filas de Propietario
            var cantidad = rp.ObtenerCantidadDeFilas();
            ViewBag.cant = (int)Math.Ceiling((double)cantidad / limite);

            // Obtener la lista de Propietario paginada
            var propietarios = rp.ObtenerPropietarios(limite, paginado);

            // Establecer la vista
            return View(propietarios);
        }



        [HttpGet]
        public IActionResult Paginado(int limite, int paginado)
        {
            var propietarios = rp.ObtenerPropietarios(limite, paginado);
            return Json(propietarios);
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

        [HttpGet]
        public ActionResult Alta(int id)
        {
            return View(rp.ObtenerPropietarioPorID(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Alta(int id, IFormCollection collection)
        {
            try
            {
                rp.AltaLogica(id);
                return RedirectToAction(nameof(Index));

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                ModelState.AddModelError(string.Empty, "Ha ocurrido un error al cargar la página.");
                return View(); // Retorna la vista con el modelo para que el usuario pueda corregir la entrada.
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
            return View();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectResult("/Home/Login");
            }
            base.OnActionExecuting(context);
        }
    }
}
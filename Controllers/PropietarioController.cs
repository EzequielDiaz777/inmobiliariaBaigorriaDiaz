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
        public ActionResult Index(int limite = 5, int paginado = 1, int cantidad = 0)
        {
            ViewBag.Id = TempData["Id"];
            ViewBag.entidad = "propietario";
            // Establece `limite` en `ViewBag` para usarlo en la vista
            ViewBag.limite = limite;

            ViewBag.rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Obtener la cantidad total de filas de Propietario
            if(cantidad == 0){
                cantidad = rp.ObtenerCantidadDeFilas();
            }

            // Calcula el número total de páginas
            ViewBag.totalPages = (int)Math.Ceiling((double)cantidad / limite);

            // Establece la página actual en `ViewBag`
            ViewBag.currentPage = paginado;
            // Devuelve la vista con los datos de `propietarios`
            return View(rp.ObtenerPropietarios(limite, paginado));
        }

        [HttpGet]
        public IActionResult Paginado(int limite, int paginado)
        {
            // Asegúrate de recibir el valor de `limite` y `paginado`
            ViewBag.limite = limite;
            return Json(rp.ObtenerPropietarios(limite, paginado));
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

        [HttpGet]
        public ActionResult BuscarPropietario(int dni)
        {
            var propietario = rp.ObtenerPropietarioPorDNI(dni);
            if (propietario != null)
            {
                // Si el inquilino se encuentra, devolvemos los datos como un objeto JSON con la propiedad "success" igual a true
                // y los datos del inquilino en la propiedad "inquilino"

                return Json(new { success = true, propietario = propietario });
            }
            else
            {
                // Si el inquilino no se encuentra, devolvemos un objeto JSON con la propiedad "success" igual a false
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public ActionResult CreateJSON([FromBody] Propietario propietario)
        {
            try
            {
                // Realizar las operaciones necesarias con el objeto propietario
                rp.AltaFisica(propietario);
                TempData["Id"] = propietario.IdPropietario;
                return Json(new { success = true, propietario.IdPropietario, message = "propietario creado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
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
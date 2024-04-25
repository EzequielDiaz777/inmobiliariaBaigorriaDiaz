using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace inmobiliariaBaigorriaDiaz.Controllers
{
    public class TipoDeInmuebleController : Controller
    {
        private RepositorioTipoDeInmueble rtdi = new RepositorioTipoDeInmueble();

        // GET: TipoDeInmueble
        [HttpGet]
        public ActionResult Index(int limite = 5, int paginado = 1, int cantidad = 0)
        {
            ViewBag.Id = TempData["Id"];
            ViewBag.entidad = "tipo de inmueble";
            ViewBag.limite = limite;

            ViewBag.rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Obtener la cantidad total de filas de Propietario
            if (cantidad == 0)
            {
                cantidad = rtdi.ObtenerCantidadDeFilas();
            }

            // Calcula el número total de páginas
            ViewBag.totalPages = (int)Math.Ceiling((double)cantidad / limite);

            // Establece la página actual en `ViewBag`
            ViewBag.currentPage = paginado;
            return View(rtdi.ObtenerTiposDeInmuebles(limite, paginado));
        }

        [HttpGet]
        public IActionResult Paginado(int limite, int paginado)
        {
            // Asegúrate de recibir el valor de `limite` y `paginado`
            ViewBag.limite = limite;
            return Json(rtdi.ObtenerTiposDeInmuebles(limite, paginado));
        }

        // GET: TipoDeInmueble/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            return View(rtdi.ObtenerTipoDeInmueblePorID(id));
        }

        // GET: TipoDeInmueble/Create
        [HttpGet]
        public ActionResult Create()
        {
            var tiposDeInmuebles = rtdi.ObtenerTiposDeInmuebles();
            ViewBag.tiposDeInmuebles = tiposDeInmuebles;
            return View();
        }

        // POST: TipoDeInmueble/Create
        [HttpPost]
        public ActionResult Create(TipoDeInmueble tipoDeInmueble)
        {
            try
            {
                rtdi.AltaFisica(tipoDeInmueble);
                TempData["Id"] = tipoDeInmueble.IdTipoDeInmueble;
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult Alta(int id)
        {
            return View(rtdi.ObtenerTipoDeInmueblePorID(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Alta(int id, IFormCollection collection)
        {
            try
            {
                rtdi.AltaLogica(id);
                return RedirectToAction(nameof(Index));

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                ModelState.AddModelError(string.Empty, "Ha ocurrido un error al cargar la página.");
                return View(); // Retorna la vista con el modelo para que el usuario pueda corregir la entrada.
            }
        }

        // GET: TipoDeInmueble/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            return View(rtdi.ObtenerTipoDeInmueblePorID(id));
        }

        // POST: TipoDeInmueble/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, TipoDeInmueble tipoDeInmueble)
        {
            try
            {
                // TODO: Add update logic here
                TipoDeInmueble tdi = rtdi.ObtenerTipoDeInmueblePorID(id);
                if (tdi != null)
                {
                    tdi.Nombre = tipoDeInmueble.Nombre;
                    tdi.Estado = tipoDeInmueble.Estado;
                    rtdi.Modificacion(tdi);
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
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int id)
        {
            return View(rtdi.ObtenerTipoDeInmueblePorID(id));
        }

        // POST: Inmueble/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                rtdi.BajaLogica(id);

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
                context.Result = new RedirectResult("/Home/Login");
            }
            base.OnActionExecuting(context);
        }
    }
}
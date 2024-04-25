using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace inmobiliariaBaigorriaDiaz.Controllers
{
    public class UsoDeInmuebleController : Controller
    {
        private RepositorioUsoDeInmueble rudi = new RepositorioUsoDeInmueble();
        // GET: UsoDeInmuebleController
        [HttpGet]
        public ActionResult Index(int limite = 5, int paginado = 1, int cantidad = 0)
        {
            ViewBag.Id = TempData["Id"];
            ViewBag.entidad = "uso de inmueble";
            ViewBag.limite = limite;
            ViewBag.rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Obtener la cantidad total de filas de Propietario
            if (cantidad == 0)
            {
                cantidad = rudi.ObtenerCantidadDeFilas();
            }

            // Calcula el número total de páginas
            ViewBag.totalPages = (int)Math.Ceiling((double)cantidad / limite);

            // Establece la página actual en `ViewBag`
            ViewBag.currentPage = paginado;
            return View(rudi.ObtenerUsosDeInmuebles(limite, paginado));
        }

        [HttpGet]
        public IActionResult Paginado(int limite, int paginado)
        {
            // Asegúrate de recibir el valor de `limite` y `paginado`
            ViewBag.limite = limite;
            return Json(rudi.ObtenerUsosDeInmuebles(limite, paginado));
        }

        // GET: UsoDeInmuebleController/Details/5
        public ActionResult Details(int id)
        {
            return View(rudi.ObtenerUsoDeInmueblePorID(id));
        }

        // GET: UsoDeInmuebleController/Create
        public ActionResult Create()
        {
            var usosDeInmuebles = rudi.ObtenerUsosDeInmuebles();
            ViewBag.usosDeInmuebles = usosDeInmuebles;
            return View();
        }

        // POST: UsoDeInmuebleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UsoDeInmueble usoDeInmueble)
        {
            try
            {
                rudi.AltaFisica(usoDeInmueble);
                TempData["Id"] = usoDeInmueble.IdUsoDeInmueble;
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
            return View(rudi.ObtenerUsoDeInmueblePorID(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Alta(int id, IFormCollection collection)
        {
            try
            {
                rudi.AltaLogica(id);
                return RedirectToAction(nameof(Index));

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                ModelState.AddModelError(string.Empty, "Ha ocurrido un error al cargar la página.");
                return View(); // Retorna la vista con el modelo para que el usuario pueda corregir la entrada.
            }
        }

        // GET: UsoDeInmuebleController/Edit/5
        public ActionResult Edit(int id)
        {
            return View(rudi.ObtenerUsoDeInmueblePorID(id));
        }

        // POST: UsoDeInmuebleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, UsoDeInmueble usoDeInmueble)
        {
            try
            {
                UsoDeInmueble? udi = rudi.ObtenerUsoDeInmueblePorID(id);
                if(udi != null){
                    udi.Nombre = usoDeInmueble.Nombre;
                    udi.Estado = usoDeInmueble.Estado;
                    rudi.Modificacion(udi);
                    return RedirectToAction(nameof(Index));
                } else {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return RedirectToAction("Edit");
            }
        }

        // GET: UsoDeInmuebleController/Delete/5
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int id)
        {
            return View(rudi.ObtenerUsoDeInmueblePorID(id));
        }

        // POST: UsoDeInmuebleController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                rudi.BajaLogica(id);
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

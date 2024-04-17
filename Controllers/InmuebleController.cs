using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace inmobiliariaBaigorriaDiaz.Controllers
{
    public class InmuebleController : Controller
    {
        private RepositorioPropietario repoP = new RepositorioPropietario();
        private RepositorioInmueble repoI = new RepositorioInmueble();
        private RepositorioTipoDeInmueble repoTI = new RepositorioTipoDeInmueble();
        private RepositorioUsoDeInmueble repoUI = new RepositorioUsoDeInmueble();
        private RepositorioInquilino repoIn = new RepositorioInquilino();

        // GET: Inmueble
        public ActionResult Index()
        {
            ViewBag.Id = TempData["Id"];
            ViewBag.entidad = "inmueble";
            return View(repoI.ObtenerInmuebles());
        }

        // GET: Inmueble/Details/5
        public ActionResult Details(int id)
        {;
            return View(repoI.ObtenerInmueblePorID(id));
        }

        // GET: Inmueble/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.TiposDeInmuebles = repoTI.ObtenerTiposDeInmuebles();
                ViewBag.Propietarios = repoP.ObtenerPropietarios();
                ViewBag.UsosDeInmuebles = repoUI.ObtenerUsosDeInmuebles();
                return View();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                ModelState.AddModelError(string.Empty, "Ha ocurrido un error al cargar la página.");
                return View();
            }
        }

        // POST: Inmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inmueble inmueble)
        {
            try
            {
                repoI.AltaFisica(inmueble);
                TempData["Id"] = inmueble.IdInmueble;
                return RedirectToAction(nameof(Index));

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                ModelState.AddModelError(string.Empty, "Ha ocurrido un error al cargar la página.");
                return View(); // Retorna la vista con el modelo para que el usuario pueda corregir la entrada.
            }
        }

        [HttpGet]
        public ActionResult Alta(int id)
        {
            return View(repoI.ObtenerInmueblePorID(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Alta(int id, IFormCollection collection)
        {
            try
            {
                repoI.AltaLogica(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                ModelState.AddModelError(string.Empty, "Ha ocurrido un error al cargar la página.");
                return View(); // Retorna la vista con el modelo para que el usuario pueda corregir la entrada.
            }
        }

        // GET: Inmueble/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.Propietarios = repoP.ObtenerPropietarios();
            ViewBag.TiposDeInmuebles = repoTI.ObtenerTiposDeInmuebles();
            ViewBag.UsosDeInmuebles = repoUI.ObtenerUsosDeInmuebles();
            return View(repoI.ObtenerInmueblePorID(id));  
        }

        // POST: Inmueble/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inmueble inmueble)
        {
            try
            {
                Inmueble? inmuebleBD = repoI.ObtenerInmueblePorID(id);
                if(inmuebleBD != null){
                    inmuebleBD.IdTipoDeInmueble= inmueble.IdTipoDeInmueble;
                    inmuebleBD.IdUsoDeInmueble = inmueble.IdUsoDeInmueble;
                    inmuebleBD.IdPropietario = inmueble.IdPropietario;
                    inmuebleBD.Direccion = inmueble.Direccion;
                    inmuebleBD.Ambientes = inmueble.Ambientes;
                    inmuebleBD.Superficie = inmueble.Superficie;
                    inmuebleBD.Longitud = inmueble.Longitud;
                    inmuebleBD.Latitud = inmueble.Latitud;
                    inmuebleBD.Precio = inmueble.Precio;
                    repoI.ModificarInmueble(inmuebleBD);
                    return RedirectToAction(nameof(Index));
                } else {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                // Maneja la excepción o imprime detalles para depuración
                Console.WriteLine(ex.ToString());
                return View();
            }
        }

        // GET: Inmueble/Delete/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int id)
        {
            return View(repoI.ObtenerInmueblePorID(id));
        }

        // POST: Inmueble/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                repoI.BajaLogica(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        /*[HttpGet]
        public IActionResult Buscar(DateTime fechaDesde, DateTime fechaHasta)
        {
            ViewBag.TiposDeInmuebles = repoTI.ObtenerTiposDeInmuebles();
            ViewBag.UsosDeInmuebles = repoUI.ObtenerUsosDeInmuebles();
            var inmueblesDisponibles = repoI.ObtenerInmueblesDisponibles(fechaDesde, fechaHasta);
            bool hayResultados = inmueblesDisponibles.Count > 0;
            return View(Tuple.Create(inmueblesDisponibles, hayResultados));
        }*/

        [HttpGet]
        public ActionResult Buscar()
        {
            ViewBag.TiposDeInmuebles = repoTI.ObtenerTiposDeInmuebles();
            ViewBag.UsosDeInmuebles = repoUI.ObtenerUsosDeInmuebles();
            return View();
        }

        // POST: /Buscar
        [HttpPost]
        public IActionResult Buscar(int IdUsoDeInmueble, int IdTipoDeInmueble, int ambientes, decimal precioDesde, decimal precioHasta, DateTime fechaDesde, DateTime fechaHasta)
        {
            var inmueblesEncontrados = repoI.BuscarInmuebles(IdUsoDeInmueble, IdTipoDeInmueble, ambientes, precioDesde, precioHasta, fechaDesde, fechaHasta);

            // Devuelve los resultados directamente a la vista utilizando el método View()
            return Json(inmueblesEncontrados);
        }

        [HttpGet]
        public IActionResult ObtenerInmueblesPorUso(int id){
            var inmuebles = repoI.ObtenerInmueblesPorUso(id);
            return Json(inmuebles);
        }

        [HttpGet]
        public IActionResult ObtenerInmueblesPorTipo(int id)
        {
            Console.WriteLine(id);
            var inmuebles = repoI.ObtenerInmueblesPorTipo(id);
            return Json(inmuebles);
        }

        [HttpGet]
        public ActionResult BuscarInquilino(int dni)
        {
            var inquilino = repoIn.ObtenerInquilinoPorDNI(dni);
            if (inquilino != null)
            {
                // Si el inquilino se encuentra, devolvemos los datos como un objeto JSON con la propiedad "success" igual a true
                // y los datos del inquilino en la propiedad "inquilino"
                
                return Json(new { success = true, inquilino = inquilino });
            }
            else
            {
                // Si el inquilino no se encuentra, devolvemos un objeto JSON con la propiedad "success" igual a false
                return Json(new { success = false });
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
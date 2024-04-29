using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

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
        public ActionResult Index(int limite = 5, int paginado = 1, int cantidad = 0)
        {
            ViewBag.Id = TempData["Id"];
            ViewBag.entidad = "inmueble";
            ViewBag.limite = limite;

            ViewBag.rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Obtener la cantidad total de filas de Propietario
            if (cantidad == 0)
            {
                cantidad = repoI.ObtenerCantidadDeFilas();
            }

            // Calcula el número total de páginas
            ViewBag.totalPages = (int)Math.Ceiling((double)cantidad / limite);

            // Establece la página actual en `ViewBag`
            ViewBag.currentPage = paginado;
            return View(repoI.ObtenerInmuebles(limite, paginado));
        }

        [HttpGet]
        public IActionResult Paginado(int limite, int paginado)
        {
            // Asegúrate de recibir el valor de `limite` y `paginado`
            ViewBag.limite = limite;
            return Json(repoI.ObtenerInmuebles(limite, paginado));
        }

        // GET: Inmueble/Details/5
        public ActionResult Details(int id)
        {
            ;
            return View(repoI.ObtenerInmueblePorID(id));
        }

        // GET: Inmueble/Create
        [HttpGet]
        public ActionResult Create(int id)
        {
            try
            {
                ViewBag.Duenio = repoP.ObtenerPropietarioPorID(id);
                Console.WriteLine(ViewBag.Duenio);
                ViewBag.TiposDeInmuebles = repoTI.ObtenerTiposDeInmuebles();
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
                if (inmuebleBD != null)
                {
                    inmuebleBD.IdTipoDeInmueble = inmueble.IdTipoDeInmueble;
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
                }
                else
                {
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

        [HttpGet]
        public ActionResult Buscar(int id)
        {
            ViewBag.IdInquilino = id;
            ViewBag.TiposDeInmuebles = repoTI.ObtenerTiposDeInmuebles();
            ViewBag.UsosDeInmuebles = repoUI.ObtenerUsosDeInmuebles();
            var inquilino = repoIn.ObtenerInquilinoPorID(id);
            ViewBag.Inquilino = inquilino;
            return View();
        }

        // POST: /Buscar
        [HttpPost]
        public IActionResult Buscar(int? IdUsoDeInmueble, int? IdTipoDeInmueble, int? ambientes, decimal? precioDesde, decimal? precioHasta, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            Console.WriteLine("Uso de inmueble: " + IdUsoDeInmueble);
            var inmueblesEncontrados = repoI.BuscarInmuebles(IdUsoDeInmueble, IdTipoDeInmueble, ambientes, precioDesde, precioHasta, fechaDesde, fechaHasta);
            return Json(inmueblesEncontrados);
        }


        [HttpGet]
        public IActionResult ObtenerInmueblesPorUso(int id)
        {
            var inmuebles = repoI.ObtenerInmueblesPorUso(id);
            return Json(inmuebles);
        }

        [HttpGet]
        public IActionResult ObtenerInmueblesPorTipo(int id)
        {
            var inmuebles = repoI.ObtenerInmueblesPorTipo(id);
            return Json(inmuebles);
        }

        [HttpGet]
        public IActionResult ListarInmuebles(int id, int limite = 5, int paginado = 1, int cantidad = 0)
        {
            ViewBag.limite = limite;

            ViewBag.rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Obtener la cantidad total de filas de Propietario
            if (cantidad == 0)
            {
                cantidad = repoI.ObtenerCantidadDeFilas();
            }

            // Calcula el número total de páginas
            ViewBag.totalPages = (int)Math.Ceiling((double)cantidad / limite);
            ViewBag.idPropietario = id;
            // Establece la página actual en `ViewBag`
            ViewBag.currentPage = paginado;
            return View(repoI.ObtenerInmueblesPorPropietario(id, limite, paginado));
        }

        [HttpGet]
        public IActionResult PaginadoListarInmuebles(int paginado, int limite)
        {
            // Asegúrate de recibir el valor de `limite` y `paginado`
            ViewBag.limite = limite;
            return Json(repoI.ObtenerInmuebles(limite, paginado));
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
                context.Result = new RedirectResult("/Home/Login");
            }
            base.OnActionExecuting(context);
        }
    }
}
using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace inmobiliariaBaigorriaDiaz.Controllers
{
    public class ContratoController : Controller
    {
        private RepositorioContrato repoC = new RepositorioContrato();
        private RepositorioInquilino repoInq = new RepositorioInquilino();
        private RepositorioInmueble repoInm = new RepositorioInmueble();
        private RepositorioRegistro rg = new RepositorioRegistro();

        // GET: Contrato
        [HttpGet]
        public ActionResult Index(int limite = 5, int paginado = 1, int cantidad = 0)
        {
            ViewBag.Id = TempData["Id"];
            ViewBag.entidad = "contrato";
            ViewBag.limite = limite;

            ViewBag.rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Obtener la cantidad total de filas de Propietario
            if (cantidad == 0)
            {
                cantidad = repoC.ObtenerCantidadDeFilas();
            }

            // Calcula el número total de páginas
            ViewBag.totalPages = (int)Math.Ceiling((double)cantidad / limite);

            // Establece la página actual en `ViewBag`
            ViewBag.currentPage = paginado;

            return View(repoC.ObtenerContratos(limite, paginado));
        }

        [HttpGet]
        public IActionResult Paginado(int limite, int paginado)
        {
            // Asegúrate de recibir el valor de `limite` y `paginado`
            ViewBag.limite = limite;
            return Json(repoC.ObtenerContratos(limite, paginado));
        }

        [HttpGet]
        public ActionResult ObtenerContratos()
        {
            var contratos = repoC.ObtenerContratos();
            return Json(contratos);
        }

        [HttpGet]
        public ActionResult ObtenerContratosVigentes()
        {
            var contratosVigentes = repoC.ObtenerContratosVigentes();
            return Json(contratosVigentes);
        }

        [HttpGet]
        public ActionResult ObtenerContratosNoVigentes()
        {
            var contratosNoVigentes = repoC.ObtenerContratosNoVigentes();
            return Json(contratosNoVigentes);
        }

        [HttpGet]
        public ActionResult ObtenerContratosPorFechas(string desde, string hasta)
        {
            var contratosPorFechas = repoC.ObtenerContratosPorFechas(desde, hasta);
            return Json(contratosPorFechas);
        }

        [HttpGet]
        public ActionResult ObtenerContratosPorDias(string dias)
        {
            var contratosPorDias = repoC.ObtenerContratosPorDias(dias);
            return Json(contratosPorDias);
        }

        // GET: Contrato/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            ViewBag.Id = TempData["Id"];
            return View(repoC.ObtenerContratoById(id));
        }

        [HttpGet]
        [Route("Contrato/Create/{idInmueble}/{idInquilino}/{precioInmueble}/{fechaDesde?}/{fechaHasta?}")]
        public ActionResult Create(int idInmueble, int idInquilino, decimal precioInmueble, DateOnly? fechaDesde, DateOnly? fechaHasta)
        {
            Inmueble? inmueble = repoInm.ObtenerInmueblePorID(idInmueble);
            Inquilino inquilino = repoInq.ObtenerInquilinoPorID(idInquilino);
            Contrato contrato = new Contrato
            {
                IdInquilino = idInquilino,
                IdInmueble = idInmueble,
                Precio = precioInmueble,
                AlquilerDesde = fechaDesde ?? default(DateOnly),
                AlquilerHasta = fechaHasta ?? default(DateOnly),
                Inquilino = inquilino,
                Inmueble = inmueble,
            };
            return View(contrato);
        }

        // POST: Contrato/CreateFromParameters
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Contrato/Create/{idInmueble}/{idInquilino}/{precioInmueble}/{fechaDesde}/{fechaHasta}")]
        public ActionResult Create(int idInmueble, int idInquilino, decimal precioInmueble, DateOnly fechaDesde, DateOnly fechaHasta)
        {
            try
            {
                // Verificar que las fechas no sean nulas
                if (fechaDesde == default || fechaHasta == default)
                {
                    // Si las fechas son nulas, devolver un mensaje de error
                    TempData["ErrorMessage"] = "Las fechas de inicio y fin de alquiler son obligatorias.";
                    return RedirectToAction(nameof(Index));
                }

                // Obtener los datos del inmueble e inquilino
                Inmueble inmueble = repoInm.ObtenerInmueblePorID(idInmueble);
                Inquilino inquilino = repoInq.ObtenerInquilinoPorID(idInquilino);

                // Crear el modelo de contrato y establecer sus propiedades
                Contrato contrato = new Contrato
                {
                    IdInquilino = idInquilino,
                    IdInmueble = idInmueble,
                    Precio = precioInmueble,
                    AlquilerDesde = fechaDesde,
                    AlquilerHasta = fechaHasta,
                    Inquilino = inquilino,
                    Inmueble = inmueble,
                };

                // Llamar al método de alta física del repositorio
                repoC.AltaFisica(contrato);

                // Registrar la acción
                DateTime horaActual = DateTime.Now;
                TimeSpan hora = new TimeSpan(horaActual.Hour, horaActual.Minute, horaActual.Second);
                var registroP = new Registro
                {
                    IdUsuario = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid)?.Value),
                    IdFila = contrato.IdContrato,
                    NombreDeTabla = "Contrato",
                    TipoDeAccion = "Alta",
                    FechaDeAccion = DateOnly.FromDateTime(DateTime.Today),
                    HoraDeAccion = hora
                };
                rg.AltaFisica(registroP);

                // Redirecciona a la acción Index
                TempData["Id"] = contrato.IdContrato;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error al crear el contrato: {ex.Message}");
                // Puedes agregar un mensaje de error a TempData para mostrarlo en la vista
                TempData["ErrorMessage"] = "Ha ocurrido un error al crear el contrato.";
                return RedirectToAction(nameof(Index));
            }
        }*/

        public ActionResult CreatePago(int idContrato, decimal monto)
        {
            // Redirigir al usuario a la acción de creación de pago en el controlador de Pago, pasando los datos necesarios en la URL
            return RedirectToAction("Create", "Pago", new { idContrato = idContrato, monto = monto });
        }


        // GET: Contrato/CreateFromParameters
        [HttpGet]
        public ActionResult Create(int id)
        {
            Contrato contrato = repoC.ObtenerContratoById(id);
            return View(contrato);
        }

        [HttpPost]
        [Route("Contrato/Create/{idInmueble}/{idInquilino}/{precioInmueble}/{fechaDesde}/{fechaHasta}")]
        public ActionResult Create(int idInmueble, int idInquilino, decimal precioInmueble, DateTime fechaDesde, DateTime fechaHasta, Contrato contrato)
        {
            try
            {
                contrato.IdInmueble = idInmueble;
                contrato.IdInquilino = idInquilino;
                contrato.Precio = precioInmueble;
                contrato.AlquilerDesde = DateOnly.FromDateTime(fechaDesde);
                contrato.AlquilerHasta = DateOnly.FromDateTime(fechaHasta);
                repoC.AltaFisica(contrato);
                TempData["Id"] = contrato.IdContrato;
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
            return View(repoC.ObtenerContratoById(id));
        }

        // GET: Contrato/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.Inmuebles = repoInm.ObtenerInmuebles();
            ViewBag.Inquilinos = repoInq.ObtenerInquilinos();
            return View(repoC.ObtenerContratoById(id));
        }

        // POST: Contrato/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Contrato contrato)
        {
            try
            {
                Contrato? contratoBD = repoC.ObtenerContratoById(id);
                if (contratoBD != null)
                {
                    contratoBD.IdInquilino = contrato.IdInquilino;
                    contratoBD.IdInmueble = contrato.IdInmueble;
                    contratoBD.Precio = contrato.Precio;
                    contratoBD.AlquilerDesde = contrato.AlquilerDesde;
                    contratoBD.AlquilerHasta = contrato.AlquilerHasta;
                    contratoBD.AlquilerHastaOriginal = contrato.AlquilerHasta;
                    repoC.ModificarContrato(contratoBD);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult CancelarContrato(int id)
        {
            ViewBag.mesesAdeudados = repoC.ObtenerMesesAdeudados(id);
            ViewBag.mesesTotales = repoC.ObtenerMesesTotales(id);
            Console.WriteLine(id);
            return View(repoC.ObtenerContratoById(id));
        }

        // GET: Contrato/Delete/5
        [HttpGet]
        public ActionResult Delete(int id)
        {
            return View(repoC.ObtenerContratoById(id));
        }

        // POST: Contrato/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                repoC.BajaFisica(id);
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
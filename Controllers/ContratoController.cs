using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace inmobiliariaBaigorriaDiaz.Controllers
{
    public class ContratoController : Controller
    {
        private RepositorioContrato repoC = new RepositorioContrato();
        private RepositorioInquilino repoInq = new RepositorioInquilino();
        private RepositorioInmueble repoInm = new RepositorioInmueble();

        // GET: Contrato
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Id = TempData["Id"];
            ViewBag.entidad = "contrato";
            return View(repoC.ObtenerContratos());
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
        [Route("Contrato/Create/{idInmueble}/{idInquilino}/{precioInmueble}/{fechaDesde}/{fechaHasta}")]
        public ActionResult Create(int idInmueble, int idInquilino, decimal precioInmueble, DateOnly fechaDesde, DateOnly fechaHasta)
        {
            Console.WriteLine(idInquilino);
            // Obtener los datos del inmueble e inquilino
            Inmueble? inmueble = repoInm.ObtenerInmueblePorID(idInmueble);
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
                // Agregar otras propiedades necesarias,
            };

            // Devolver la vista con el modelo de contrato
            return View(contrato);
        }

        // POST: Contrato/CreateFromParameters
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Contrato/Create/{idInmueble}/{idInquilino}/{precioInmueble}/{fechaDesde}/{fechaHasta}")]
        public ActionResult Create(Contrato contrato)
        {
            try
            {
                contrato.AlquilerHastaOriginal = contrato.AlquilerHasta;
                repoC.AltaFisica(contrato);
                TempData["Id"] = contrato.IdContrato;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult CreatePago(int idContrato, decimal monto)
        {
            // Redirigir al usuario a la acción de creación de pago en el controlador de Pago, pasando los datos necesarios en la URL
            return RedirectToAction("Create", "Pago", new { idContrato = idContrato, monto = monto });
        }


        // GET: Contrato/CreateFromParameters
        [HttpGet]

        public ActionResult Create(int id)
        {

            // Crear el modelo de contrato y establecer sus propiedades
            Contrato contrato = repoC.ObtenerContratoById(id);


            // Devolver la vista con el modelo de contrato
            return View(contrato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contrato contrato, IFormCollection collection)
        {
            try
            {
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
                context.Result = RedirectToAction("Index", "Home"); // Redirige al Index del controlador Home
            }
            base.OnActionExecuting(context);
        }
    }
}
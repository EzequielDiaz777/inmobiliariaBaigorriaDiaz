using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace inmobiliariaBaigorriaDiaz.Controllers
{
    public class PagoController : Controller
    {
        private RepositorioPago repoP = new RepositorioPago();
        private RepositorioContrato repoCont = new RepositorioContrato();
        private RepositorioRegistro rg = new RepositorioRegistro();

        // GET: Pago
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Id = TempData["Id"];
            ViewBag.entidad = "pago";
            return View(repoP.ObtenerPagos());
        }

        // GET: Pago/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            ViewBag.Id = TempData["Id"];
            return View(repoP.ObtenerPagoPorId(id));
        }

        // GET: Pago/Create
        [HttpGet]
        public ActionResult Create(int idContrato, decimal monto)
        {
            ViewBag.IdContrato = idContrato;
            ViewBag.Monto = monto;
            string[] meses = { "enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre", "cancelación" };
            ViewBag.Meses = meses;
            return View();
        }

        // POST: Pago/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pago pago)
        {
            try
            {
                repoP.AltaFisica(pago);
                TempData["Id"] = pago.NumeroDePago;
                DateTime horaActual = DateTime.Now;
                TimeSpan hora = new TimeSpan(horaActual.Hour, horaActual.Minute, horaActual.Second);

                var registro = new Registro
                {
                    IdUsuario = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid)?.Value),
                    IdFila = pago.NumeroDePago,
                    NombreDeTabla = "Pago",
                    TipoDeAccion = "Alta",
                    FechaDeAccion = DateOnly.FromDateTime(DateTime.Today),
                    HoraDeAccion = hora
                };
                rg.AltaFisica(registro);
                return RedirectToAction("Index", "Contrato");
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult Alta(int id)
        {
            return View(repoP.ObtenerPagoPorId(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Alta(int id, IFormCollection collection)
        {
            try
            {
                repoP.AltaLogica(id);
                DateTime horaActual = DateTime.Now;
                TimeSpan hora = new TimeSpan(horaActual.Hour, horaActual.Minute, horaActual.Second);

                var registroP = new Registro
                {
                    IdUsuario = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid)?.Value),
                    IdFila = id,
                    NombreDeTabla = "Pago",
                    TipoDeAccion = "Alta",
                    FechaDeAccion = DateOnly.FromDateTime(DateTime.Today),
                    HoraDeAccion = hora
                };
                return RedirectToAction(nameof(Index));

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                ModelState.AddModelError(string.Empty, "Ha ocurrido un error al cargar la página.");
                return View(); // Retorna la vista con el modelo para que el usuario pueda corregir la entrada.
            }
        }

        // GET: Pago/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.Contratos = repoCont.ObtenerContratos();
            string[] meses = ["enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre", "cancelación" ];
            ViewBag.Meses = meses;
            return View(repoP.ObtenerPagoPorId(id));
        }

        // POST: Pago/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Pago pago)
        {
            try
            {
                Pago pagoBD = repoP.ObtenerPagoPorId(id);
                if (pagoBD != null){
                    pagoBD.MesDePago = pago.MesDePago;
                    repoP.ModificarPago(pagoBD);
                    return RedirectToAction(nameof(Index));
                } else {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult ConfirmarPago(int id)
        {
            decimal deuda;
            var mesesPagados = repoCont.ObtenerMesesPagados(id);
            var mesesTotales = repoCont.ObtenerMesesTotales(id);
            var mesesPagos = mesesPagados < (int)(mesesTotales/2);
            // Obtiene el contrato y calcula la deuda (esto es un ejemplo, personaliza según tu lógica)
            var contrato = repoCont.ObtenerContratoById(id);
            if(mesesPagos){
                deuda = contrato.Precio*2; // Por ejemplo, el precio del contrato es la deuda
            } else {
                deuda = contrato.Precio;
            }

            var pago = new Pago
            {
                IdContrato = contrato.IdContrato,
                Contrato = contrato,
                MesDePago = "cancelacion",
                Monto = deuda,
                Fecha = DateOnly.FromDateTime(DateTime.Today)
            };
            Console.WriteLine(pago.IdContrato);
            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmarPago(Pago pago)
        {
            Console.WriteLine(pago.IdContrato);
            repoP.AltaFisica(pago);
            TempData["Id"] = pago.NumeroDePago;
            DateTime horaActual = DateTime.Now;
            TimeSpan hora = new TimeSpan(horaActual.Hour, horaActual.Minute, horaActual.Second);

            var registroP = new Registro
            {
                IdUsuario = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid)?.Value),
                IdFila = pago.NumeroDePago,
                NombreDeTabla = "Pago",
                TipoDeAccion = "Alta",
                FechaDeAccion = DateOnly.FromDateTime(DateTime.Today),
                HoraDeAccion = hora
            };
            rg.AltaFisica(registroP);
            
            var registroC = new Registro
            {
                IdUsuario = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid)?.Value),
                IdFila = pago.IdContrato,
                NombreDeTabla = "Contrato",
                TipoDeAccion = "Baja",
                FechaDeAccion = DateOnly.FromDateTime(DateTime.Today),
                HoraDeAccion = hora
            };
            rg.AltaFisica(registroC);
            var contrato = repoCont.ObtenerContratoById(pago.IdContrato);
            contrato.AlquilerHasta = DateOnly.FromDateTime(DateTime.Now);
            repoCont.ModificarContrato(contrato);
            // Redirige al índice de contratos después de procesar el pago
            return RedirectToAction("Index", "Contrato");
        }

        // GET: Pago/Delete/5
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int id)
        {
            return View(repoP.ObtenerPagoPorId(id));
        }

        // POST: Pago/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                repoP.BajaLogica(id);
                DateTime horaActual = DateTime.Now;
                TimeSpan hora = new TimeSpan(horaActual.Hour, horaActual.Minute, horaActual.Second);

                var registro = new Registro
                {
                    IdUsuario = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid)?.Value),
                    IdFila = id,
                    NombreDeTabla = "Pago",
                    TipoDeAccion = "Baja",
                    FechaDeAccion = DateOnly.FromDateTime(DateTime.Today),
                    HoraDeAccion = hora
                };
                rg.AltaFisica(registro);
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
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
            return View(repoP.ObtenerPagoById(id));
        }

        // GET: Pago/Create
        [HttpGet]
        public ActionResult Create(int idContrato, decimal monto)
        {
            ViewBag.IdContrato = idContrato;
            ViewBag.Monto = monto;
            string[] meses = { "enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre" };
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

        // GET: Pago/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.Contratos = repoCont.ObtenerContratos();
            string[] meses = ["enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre"];
            ViewBag.Meses = meses;
            return View(repoP.ObtenerPagoById(id));
        }

        // POST: Pago/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Pago pago)
        {
            try
            {
                Pago pagoBD = repoP.ObtenerPagoById(id);
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

        // GET: Pago/Delete/5
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int id)
        {
            return View(repoP.ObtenerPagoById(id));
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
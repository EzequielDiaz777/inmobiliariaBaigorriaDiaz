using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliariaBaigorriaDiaz.Controllers
{
    public class PagoController : Controller
    {
        private RepositorioPago repoP = new RepositorioPago();
        private RepositorioContrato repoCont = new RepositorioContrato();
        

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
        public ActionResult Create()
        {
            ViewBag.Contratos = repoCont.ObtenerContratos();
            string[] meses = ["enero","febrero","marzo","abril","mayo","junio","julio","agosto","septiembre","octubre","noviembre","diciembre"];
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
                return RedirectToAction(nameof(Index));
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
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
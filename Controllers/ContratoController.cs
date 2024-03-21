using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliariaBaigorriaDiaz.Controllers
{
    public class ContratoController : Controller
    {
        private RepositorioContrato rc = new RepositorioContrato();
        private RepositorioInquilino rinq = new RepositorioInquilino();
        private RepositorioInmueble rinm = new RepositorioInmueble();

        // GET: Contrato
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Id = TempData["Id"];
            ViewBag.entidad = "contrato";
            return View(rc.ObtenerContratos());
        }

        // GET: Contrato/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            ViewBag.Id = TempData["Id"];
            return View(rc.ObtenerContratoById(id));
        }

        // GET: Contrato/Create
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Inmuebles = rinm.ObtenerInmuebles();
            ViewBag.Inquilinos = rinq.ObtenerInquilinos();
            return View();
        }

        // POST: Contrato/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contrato contrato)
        {
            try
            {
                rc.AltaFisica(contrato);
                TempData["Id"] = contrato.IdContrato;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Contrato/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            return View(rc.ObtenerContratoById(id));
        }

        // POST: Contrato/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Contrato/Delete/5
        [HttpGet]
        public ActionResult Delete(int id)
        {
            return View(rc.ObtenerContratoById(id));
        }

        // POST: Contrato/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Contrato contrato)
        {
            try
            {
                rc.BajaLogica(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
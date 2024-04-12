using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Mvc;

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

        // GET: Contrato/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            ViewBag.Id = TempData["Id"];
            return View(repoC.ObtenerContratoById(id));
        }

        // GET: Contrato/Create
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Inmuebles = repoInm.ObtenerInmuebles();
            ViewBag.Inquilinos = repoInq.ObtenerInquilinos();
            return View();
        }

        // POST: Contrato/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contrato contrato)
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
                if (contratoBD != null){
                    contratoBD.IdInquilino = contrato.IdInquilino;
                    contratoBD.IdInmueble = contrato.IdInmueble;
                    contratoBD.Precio = contrato.Precio;
                    contratoBD.AlquilerDesde = contrato.AlquilerDesde;
                    contratoBD.AlquilerHasta = contrato.AlquilerHasta;
                    repoC.ModificarContrato(contratoBD);
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
                repoC.BajaLogica(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
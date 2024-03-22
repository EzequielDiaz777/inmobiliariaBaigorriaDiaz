using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliariaBaigorriaDiaz.Controllers
{
    public class TipoDeInmuebleController : Controller
    {
        private RepositorioTipoDeInmueble rtdi = new RepositorioTipoDeInmueble();

        // GET: TipoDeInmueble
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Id = TempData["Id"];
            ViewBag.entidad = "tipo de inmueble";
            return View(rtdi.ObtenerTiposDeInmuebles());
        }

        // GET: TipoDeInmueble/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            return View(rtdi.ObtenerTipoDeInmueblePorID(id));
        }

        // GET: TipoDeInmueble/Create
        [HttpGet]
        public ActionResult Create()
        {
            var tiposDeInmuebles = rtdi.ObtenerTiposDeInmuebles();
            ViewBag.tiposDeInmuebles = tiposDeInmuebles;
            return View();
        }

        // POST: TipoDeInmueble/Create
        [HttpPost]
        public ActionResult Create(TipoDeInmueble tipoDeInmueble)
        {
            try
            {
                rtdi.AltaFisica(tipoDeInmueble);
                TempData["Id"] = tipoDeInmueble.IdTipoDeInmueble;
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: TipoDeInmueble/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            return View(rtdi.ObtenerTipoDeInmueblePorID(id));
        }

        // POST: TipoDeInmueble/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, TipoDeInmueble tipoDeInmueble)
        {
            try
            {
                // TODO: Add update logic here
                TipoDeInmueble tdi = rtdi.ObtenerTipoDeInmueblePorID(id);
                if (tdi != null)
                {
                    tdi.Nombre = tipoDeInmueble.Nombre;
                    Console.WriteLine(tdi.Nombre);
                    tdi.Estado = tipoDeInmueble.Estado;
                    Console.WriteLine(tdi.Estado);
                    rtdi.Modificacion(tdi);
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return RedirectToAction("Edit");
            }
        }

        public ActionResult Delete(int id)
        {
            return View(rtdi.ObtenerTipoDeInmueblePorID(id));
        }

        // POST: Inmueble/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                rtdi.BajaLogica(id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
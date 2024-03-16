using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliariaBaigorriaDiaz.Controllers
{
    public class InmuebleController : Controller
    {
        private RepositorioInmueble ri = new RepositorioInmueble();
        private RepositorioPropietario rp = new RepositorioPropietario();
        private RepositorioTipoDeInmueble rtdi = new RepositorioTipoDeInmueble();

        // GET: Inmueble
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Id = TempData["Id"];
            ViewBag.entidad = "inmueble";
            return View(ri.ObtenerInmuebles());
        }

        // GET: Inmueble/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            return View(ri.ObtenerInmueblePorID(id));
        }

        // GET: Inmueble/Create
        [HttpGet]
        public ActionResult Create()
        {
            var propietarios = rp.ObtenerPropietarios();
            var inmuebles = rtdi.ObtenerTiposDeInmuebles();
            ViewBag.propietarios = propietarios;
            ViewBag.inmuebles = inmuebles;
            return View();
        }

        // POST: Inmueble/Create
        [HttpPost]
        public ActionResult Create(Inmueble inmueble)
        {
            try
            {
                ri.AltaFisica(inmueble);
                TempData["Id"] = inmueble.IdInmueble;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Inmueble/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            string[] lista = new string[3];
            lista[0] = "Casa";
            lista[1] = "Departamento";
            lista[2] = "Oficina";
            ViewBag.lista = lista;
            ViewBag.propietarios = rp.ObtenerPropietarios();
            ViewBag.inmuebles = rtdi.ObtenerTiposDeInmuebles();
            return View(ri.ObtenerInmueblePorID(id));
        }

        // POST: Inmueble/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Inmueble inmueble)
        {
            try
            {
                // TODO: Add update logic here
                Inmueble? i = ri.ObtenerInmueblePorID(id);
                if (i != null)
                {
                    i.IdTipoDeInmueble = inmueble.IdTipoDeInmueble;
                    i.Direccion = inmueble.Direccion;
                    i.Ambientes = inmueble.Ambientes;
                    i.Superficie = inmueble.Superficie;
                    i.Latitud = inmueble.Latitud;
                    i.Longitud = inmueble.Longitud;
                    i.Precio = inmueble.Precio;
                    i.Estado = inmueble.Estado;
                    i.IdPropietario = inmueble.IdPropietario;
                    ri.Modificacion(i);
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

        // GET: Inmueble/Delete/5
        [HttpGet]
        public ActionResult Delete(int id)
        {
            return View(ri.ObtenerInmueblePorID(id));
        }

        // POST: Inmueble/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                ri.BajaFisica(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
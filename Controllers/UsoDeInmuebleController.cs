using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace inmobiliariaBaigorriaDiaz.Controllers
{
    public class UsoDeInmuebleController : Controller
    {
        private RepositorioUsoDeInmueble rudi = new RepositorioUsoDeInmueble();
        // GET: UsoDeInmuebleController
        public ActionResult Index()
        {
            ViewBag.Id = TempData["Id"];
            ViewBag.entidad = "uso de inmueble";
            return View(rudi.ObtenerUsosDeInmuebles());
        }

        // GET: UsoDeInmuebleController/Details/5
        public ActionResult Details(int id)
        {
            return View(rudi.ObtenerUsoDeInmueblePorID(id));
        }

        // GET: UsoDeInmuebleController/Create
        public ActionResult Create()
        {
            var usosDeInmuebles = rudi.ObtenerUsosDeInmuebles();
            ViewBag.usosDeInmuebles = usosDeInmuebles;
            return View();
        }

        // POST: UsoDeInmuebleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UsoDeInmueble usoDeInmueble)
        {
            try
            {
                rudi.AltaFisica(usoDeInmueble);
                TempData["Id"] = usoDeInmueble.IdUsoDeInmueble;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsoDeInmuebleController/Edit/5
        public ActionResult Edit(int id)
        {
            return View(rudi.ObtenerUsoDeInmueblePorID(id));
        }

        // POST: UsoDeInmuebleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, UsoDeInmueble usoDeInmueble)
        {
            try
            {
                UsoDeInmueble? udi = rudi.ObtenerUsoDeInmueblePorID(id);
                if(udi != null){
                    udi.Nombre = usoDeInmueble.Nombre;
                    udi.Estado = usoDeInmueble.Estado;
                    rudi.Modificacion(udi);
                    return RedirectToAction(nameof(Index));
                } else {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return RedirectToAction("Edit");
            }
        }

        // GET: UsoDeInmuebleController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(rudi.ObtenerUsoDeInmueblePorID(id));
        }

        // POST: UsoDeInmuebleController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                rudi.BajaLogica(id);
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

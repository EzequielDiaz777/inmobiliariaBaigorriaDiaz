using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace inmobiliariaBaigorriaDiaz.Controllers
{
    public class InquilinoController : Controller
    {
        private RepositorioInquilino ri = new RepositorioInquilino();

        // GET: Inquilino
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Id = TempData["Id"];
            ViewBag.entidad = "inquilino";
            return View(ri.ObtenerInquilinos());
        }

        // GET: Inquilino/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            return View(ri.ObtenerInquilinoPorID(id));
        }

        // GET: Inquilino/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Inquilino/Create
        [HttpPost]
        public ActionResult Create(Inquilino inquilino)
        {
            try
            {
                ri.AltaFisica(inquilino);
                TempData["Id"] = inquilino.IdInquilino;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Inquilino/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            return View(ri.ObtenerInquilinoPorID(id));
        }

        // POST: Inquilino/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Inquilino inquilino)
        {
            try
            {
                // TODO: Add update logic here
                Inquilino i = ri.ObtenerInquilinoPorID(id);
                if (i != null)
                {
                    i.DNI = inquilino.DNI;
                    i.Nombre = inquilino.Nombre;
                    i.Apellido = inquilino.Apellido;
                    i.Email = inquilino.Email;
                    i.Telefono = inquilino.Telefono;
                    i.Estado = inquilino.Estado;
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

        // GET: Inquilino/Delete/5
        [HttpGet]
        public ActionResult Delete(int id)
        {
            return View(ri.ObtenerInquilinoPorID(id));
        }

        // POST: Inquilino/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add Delete logic here
                ri.BajaLogica(id);
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
using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliariaBaigorriaDiaz.Controllers
{
    [Authorize("Administrador")]
    public class RegistroController : Controller
    {
        private RepositorioRegistro rg = new RepositorioRegistro();
        // GET: RegistroController
        public ActionResult Index()
        {
            return View(rg.ObtenerRegistros());
        }

        // GET: RegistroController/Details/5
        public ActionResult Details(int id)
        {
            return View(rg.ObtenerRegistroPorID(id));
        }

        // POST: RegistroController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: RegistroController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RegistroController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace inmobiliariaBaigorriaDiaz.Controllers
{
    public class UsoDeInmuebleController : Controller
    {
        // GET: UsoDeInmuebleController
        public ActionResult Index()
        {
            return View();
        }

        // GET: UsoDeInmuebleController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UsoDeInmuebleController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsoDeInmuebleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: UsoDeInmuebleController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UsoDeInmuebleController/Edit/5
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

        // GET: UsoDeInmuebleController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UsoDeInmuebleController/Delete/5
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

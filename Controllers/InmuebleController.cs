using inmobiliariaBaigorriaDiaz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliariaBaigorriaDiaz.Controllers
{
    public class InmuebleController : Controller
    {
        private RepositorioPropietario repoP = new RepositorioPropietario();
        private RepositorioInmueble repoI = new RepositorioInmueble();
        private RepositorioTipoDeInmueble repoTI = new RepositorioTipoDeInmueble();
        // GET: Inmueble
        public ActionResult Index()
        {
            ViewBag.Id = TempData["Id"];
            ViewBag.entidad = "inmueble";
            return View(repoI.ObtenerInmuebles());
        }

        // GET: Inmueble/Details/5
        public ActionResult Details(int id)
        {
            return View(repoI.ObtenerInmueblePorID(id));
        }

        // GET: Inmueble/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.Propietarios = repoP.ObtenerPropietarios();
                ViewBag.TiposDeInmuebles = repoTI.ObtenerTiposDeInmuebles();
                return View();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                ModelState.AddModelError(string.Empty, "Ha ocurrido un error al cargar la página.");
                return View();
            }
        }

        // POST: Inmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inmueble inmueble)
        {
            try
            {
                repoI.Alta(inmueble);
                TempData["Id"] = inmueble.IdInmueble;
                return RedirectToAction(nameof(Index));

            }
            catch (System.Exception)
            {
                return View(); // Retorna la vista con el modelo para que el usuario pueda corregir la entrada.
            }
        }

        // GET: Inmueble/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var aux = repoI.ObtenerInmueblePorID(id);
                ViewBag.Propietarios = repoP.ObtenerPropietarios();
                ViewBag.TiposDeInmuebles = repoTI.ObtenerTiposDeInmuebles();
                ViewBag.NombreDelTipoDeInmuebleSeleccionado = aux.Tipo.Nombre;
                ViewBag.UsoSeleccionado = aux.Uso.ToString();
                ViewBag.IdPropietarioSeleccionado = aux.IdPropietario;
                return View(aux);
            }
            catch (System.Exception)
            {
                return View();
            }
        }


        // POST: Inmueble/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inmueble inmueble)
        {
            try
            {
                Inmueble i = repoI.ObtenerInmueblePorID(id);

                i.Tipo = inmueble.Tipo;
                i.Direccion = inmueble.Direccion;
                i.Ambientes = inmueble.Ambientes;
                i.Superficie = inmueble.Superficie;
                i.Longitud = inmueble.Longitud;
                i.Latitud = inmueble.Latitud;
                i.Precio = inmueble.Precio;
                i.Uso = inmueble.Uso;
                i.Estado = inmueble.Estado;
                i.Duenio = inmueble.Duenio;

                repoI.ModificarInmueble(i);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Maneja la excepción o imprime detalles para depuración
                Console.WriteLine(ex.ToString());
                return View();
            }
        }

        // GET: Inmueble/Delete/5
        public ActionResult Delete(int id)
        {
            return View(repoI.ObtenerInmueblePorID(id));
        }

        // POST: Inmueble/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                repoI.EliminarInmueble(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
using inmobiliariaBaigorriaDiaz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace inmobiliariaBaigorriaDiaz.Controllers
{
    public class ContratoController : Controller
    {
        private RepositorioContrato repoC = new RepositorioContrato();
        private RepositorioInquilino repoInq = new RepositorioInquilino();
        private RepositorioInmueble repoInm = new RepositorioInmueble();
        private RepositorioRegistro rg = new RepositorioRegistro();

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Contrato/Create/{idInmueble}/{idInquilino}/{precioInmueble}/{fechaDesde}/{fechaHasta}")]
        public ActionResult Create(Contrato contrato)
        {
            try
            {
                repoC.AltaFisica(contrato);
                TempData["Id"] = contrato.IdContrato;
                // Crear el registro de la operación
                var registro = new Registro{
                    IdUsuario = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid)?.Value), 
                    IdFila = contrato.IdContrato, 
                    NombreDeTabla = "Contrato", 
                    TipoDeAccion = "Alta", 
                    FechaDeAccion = DateOnly.FromDateTime(DateTime.Today)
                };
                Console.WriteLine(registro.IdUsuario);
                rg.AltaFisica(registro);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Contrato/CreateFromParameters
        [HttpGet]
        [Route("Contrato/Create/{idInmueble}/{idInquilino}/{precioInmueble}/{fechaDesde}/{fechaHasta}")]
        public ActionResult Create(int idInmueble, int idInquilino, decimal precioInmueble, DateOnly fechaDesde, DateOnly fechaHasta)
        {
            // Obtener los datos del inmueble e inquilino
            Inmueble? inmueble = repoInm.ObtenerInmueblePorID(idInmueble);
            Inquilino inquilino = repoInq.ObtenerInquilinoPorID(idInquilino);

            // Crear el modelo de contrato y establecer sus propiedades
            Contrato contrato = new Contrato
            {
                IdInquilino = idInquilino,
                IdInmueble = idInmueble,
                Precio = precioInmueble,
                AlquilerDesde = fechaDesde,
                AlquilerHasta = fechaHasta,
                Inquilino = inquilino,
                Inmueble = inmueble,
                // Agregar otras propiedades necesarias,
            };

            // Devolver la vista con el modelo de contrato
            return View(contrato);
        }


        //return RedirectToAction("Create", new { nombreCompleto = nombreCompleto, inmueble = inmueble, direccion = inmueble.Direccion, inquilino = inquilino, precioInmueble = precioInmueble, fechaDesde = fechaDesde, fechaHasta = fechaHasta });

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
                if (contratoBD != null)
                {
                    contratoBD.IdInquilino = contrato.IdInquilino;
                    contratoBD.IdInmueble = contrato.IdInmueble;
                    contratoBD.Precio = contrato.Precio;
                    contratoBD.AlquilerDesde = contrato.AlquilerDesde;
                    contratoBD.AlquilerHasta = contrato.AlquilerHasta;
                    repoC.ModificarContrato(contratoBD);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
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
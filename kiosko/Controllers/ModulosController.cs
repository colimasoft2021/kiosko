#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using kiosko.Models;
using Microsoft.AspNetCore.Authorization;

namespace kiosko.Controllers
{
    public class ModulosController : Controller
    {
        private readonly KioskoCmsContext _context;

        public ModulosController(KioskoCmsContext context)
        {
            _context = context;
        }

        // GET: Modulos
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Modulos.ToListAsync());
        }

        [Authorize]
        // GET: Modulos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _context.Modulos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        public JsonResult GetAllModulos()
        {
            var modulos = _context.Modulos.OrderBy(c => c.Orden);
            return Json(modulos);
        }
        // GET: Modulos/GetModulosComponentsForApp
        public JsonResult GetModulosAndComponentsForApp()
        {
            var modulos = _context.Modulos.OrderBy(c => c.Orden)
                .Include(m => m.Componentes)
                    .ThenInclude(m => m.Desplazantes)
                    .ToList();
            return Json(modulos);
        }

        [HttpPost()]
        public IActionResult SaveMenuModulo(Modulo modulo)
        {
            IActionResult ret = null;
            _context.Add(modulo);
            _context.SaveChanges();

            ret = StatusCode(StatusCodes.Status201Created, modulo);
            return ret;
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> updateModulo()
        {
            var idModulo = Int32.Parse(Request.Form["idModulo"]);
            var modulo = await _context.Modulos
                .FirstOrDefaultAsync(m => m.Id == idModulo);
            modulo.Titulo = Request.Form["tituloModulo"];
            modulo.TiempoInactividad = Int32.Parse(Request.Form["tiempoInactividad"]);

            IActionResult ret = null;
            _context.Update(modulo);
            _context.SaveChanges();

            ret = StatusCode(StatusCodes.Status201Created, modulo);
            return ret;
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public IActionResult deleteModulo(int id)
        {
            var modulo = _context.Modulos.Find(id);
            IActionResult ret = null;
            _context.Modulos.Remove(modulo);
            _context.SaveChanges();

            ret = StatusCode(StatusCodes.Status201Created, modulo);
            return ret;
        }

        /*
        public JsonResult GetModulosAndProgressByUser()
        {
            var dataUser = _context.Usuarios.Where(u => u.IdUsuario == 1);
            foreach(var u in dataUser)
            {
                var modulos = _context.Modulos.OrderBy(c => c.Orden).Load();
            }
        }
        */
    }
}

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
            var modulos = _context.Modulos.Where(m => m.Id > 1).OrderBy(m => m.Orden);
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
        public IActionResult GetModulosAndProgressByUser([FromBody] Usuario usuario)
        {
            var modulos = _context.Modulos;
            foreach (var m in modulos)
            {
                _context.Progresos.Where(p => p.IdModulo == m.Id).Where(p => p.IdUsuario == usuario.IdUsuario).Load();
            }
            return Json(modulos);
        }
    }
}
